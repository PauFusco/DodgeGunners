using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class Player
    {
        public enum Type
        {
            DEFAULT = 0,
            HOST,
            REMOTE
        }

        public Player(string username, EndPoint endpoint, Type type, Socket socket)
        {
            _username = username;
            _endpoint = endpoint;
            _usedSocket = socket;
            _type = type;
        }

        public string GetUsername()
        { return _username; }

        public EndPoint GetEndPoint()
        { return _endpoint; }

        public Type GetPlayerType()
        { return _type; }

        public Socket GetSocket()
        { return _usedSocket; }

        private readonly string _username;
        private readonly EndPoint _endpoint;
        private readonly Socket _usedSocket;
        private readonly Type _type;
    }

    private Player remote;
    private Player local;

    private void Awake()
    { DontDestroyOnLoad(transform.gameObject); }

    public void AddRemote(string username, EndPoint ep, Player.Type type, Socket socket)
    { remote = new(username, ep, type, socket); }

    public void SetLocal(string username, Player.Type type)
    { local = new(username, null, type, null); }

    public Player GetRemote()
    { return remote; }

    public Player GetLocal()
    { return local; }

    public void ClearRemote()
    { remote = new("", null, 0, null); }
}