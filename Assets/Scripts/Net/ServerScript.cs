using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerScript : MonoBehaviour
{
    private class Room
    {
        public Room()
        {
            _roomcode = (UInt16)UnityEngine.Random.Range(0, 65535);
            _players = new List<Player>();
        }

        public void AddPlayer(string username, IPEndPoint ipep)
        {
            Player newPlayer = new(username, ipep);
            _players.Add(newPlayer);
        }

        public int GetSize()
        { return _players.Count; }

        public UInt16 GetCode()
        { return _roomcode; }

        public List<Player> _players;
        private readonly UInt16 _roomcode;
    }

    private class Player
    {
        public Player(string username, IPEndPoint ipep)
        {
            _username = username;
            _ipendpoint = ipep;
        }

        public string GetUsername()
        { return _username; }

        public IPEndPoint GetIPEndPoint()
        { return _ipendpoint; }

        private readonly string _username;
        private readonly IPEndPoint _ipendpoint;
    }

    private readonly List<Room> rooms = new();

    private string newplayerusername;
    private UInt16 newplayerroomcode;
    private IPEndPoint newplayerep;
    private bool roompending;
    private bool remotepending;

    private Socket socket;

    private void Start()
    {
        roompending = false;
        remotepending = false;

        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        Thread newConnectionsCheck = new(CheckConnections);
        newConnectionsCheck.Start();
    }

    private void Update()
    {
        if (roompending)
        {
            Room newRoom = CreateNewRoom(newplayerusername, newplayerep);

            socket.SendTo(Encoding.ASCII.GetBytes(newRoom.GetCode().ToString()), newplayerep);

            roompending = false;
        }

        if (remotepending)
        {
            foreach (var room in rooms)
            {
                if (room.GetCode() == newplayerroomcode)
                {
                    room.AddPlayer(newplayerusername, newplayerep);

                    SendUsernamesIPsToPlayers(room);

                    remotepending = false;
                }
            }
        }
    }

    private void CheckConnections()
    {
        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (true)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            MemoryStream message = new(data);
            BinaryReader messageBR = new(message);

            LobbyMessageType messageType = (LobbyMessageType)messageBR.ReadInt32();

            switch (messageType)
            {
                case LobbyMessageType.CREATE:
                    newplayerusername = messageBR.ReadString();
                    newplayerep = (IPEndPoint)remote;

                    roompending = true;

                    break;

                case LobbyMessageType.JOIN:
                    newplayerusername = messageBR.ReadString();
                    newplayerroomcode = UInt16.Parse(messageBR.ReadString());
                    newplayerep = (IPEndPoint)remote;

                    remotepending = true;

                    break;

                case LobbyMessageType.DEFAULT:
                    break;
            }
        }
    }

    private Room CreateNewRoom(string hostUsername, IPEndPoint hostIPEP)
    {
        Room newRoom = new();
        rooms.Add(newRoom);

        newRoom.AddPlayer(hostUsername, hostIPEP);

        return newRoom;
    }

    private void SendUsernamesIPsToPlayers(Room room)
    {
        Player host = room._players[0];
        Player remote = room._players[1];

        MemoryStream hostMS = new();
        BinaryWriter hostMSBW = new(hostMS);
        hostMSBW.Write(host.GetUsername());
        hostMSBW.Write(host.GetIPEndPoint().Address.ToString());

        socket.SendTo(hostMS.ToArray(), remote.GetIPEndPoint());

        MemoryStream remoteMS = new();
        BinaryWriter remoteMSBW = new(remoteMS);
        remoteMSBW.Write(remote.GetUsername());
        remoteMSBW.Write(remote.GetIPEndPoint().Address.ToString());

        socket.SendTo(remoteMS.ToArray(), host.GetIPEndPoint());
    }
}