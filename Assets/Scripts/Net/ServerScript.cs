using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private IPEndPoint _ipendpoint;
    }

    private Socket socket;
    private Room room;
    private bool hostOnline;

    private void Start()
    {
        room = new();

        hostOnline = false;

        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);
    }

    private void Update()
    {
        Thread newConnectionsCheck = new(CheckConnections);
        newConnectionsCheck.Start();
    }

    private void CheckConnections()
    {
        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (room.GetSize() < 2)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            IPEndPoint ipep = (IPEndPoint)remote;

            room.AddPlayer(Encoding.ASCII.GetString(data, 0, recv), ipep);

            if (!hostOnline)
            {
                byte[] roomCode = Encoding.ASCII.GetBytes(room.GetCode().ToString());
                socket.SendTo(roomCode, remote);
                hostOnline = true;
            }
            else
            {
                byte[] hostIP = Encoding.ASCII.GetBytes(room._players[0].GetIPEndPoint().Address.ToString());
                byte[] guestIP = Encoding.ASCII.GetBytes(room._players[1].GetIPEndPoint().Address.ToString());

                socket.SendTo(guestIP, room._players[0].GetIPEndPoint());
                socket.SendTo(hostIP, room._players[1].GetIPEndPoint());
            }
        }
    }
}