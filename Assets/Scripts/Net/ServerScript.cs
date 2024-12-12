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
        }

        public void AddPlayer(string username, IPAddress address)
        {
            Player newPlayer = new(username, address);
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
        public Player(string username, IPAddress address)
        {
            _username = username;
            _address = address;
        }

        public string GetUsername()
        { return _username; }

        private string _username;
        private IPAddress _address;
    }

    private Socket socket;
    private Room room;

    // Start is called before the first frame update
    private void Start()
    {
        room = new();

        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);
    }

    // Update is called once per frame
    private void Update()
    {
        Thread newConnectionsCheck = new(CheckConnections);
        newConnectionsCheck.Start();
    }

    private void CheckConnections()
    {
        bool hostOnline = false;

        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (room.GetSize() < 2)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            IPEndPoint ipep = (IPEndPoint)remote;

            room.AddPlayer(Encoding.ASCII.GetString(data, 0, recv), ipep.Address);

            if (!hostOnline)
            {
                byte[] roomCode = Encoding.ASCII.GetBytes(room.GetCode().ToString());
                socket.SendTo(roomCode, remote);
                hostOnline = true;
            }
            else
            {
            }
        }
    }
}