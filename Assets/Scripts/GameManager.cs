using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class NetPlayer
    {
        public enum Type
        {
            DEFAULT = 0,
            HOST,
            REMOTE
        }

        public NetPlayer(string username, EndPoint endpoint, Type type, Socket socket)
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

        public UInt16 GetScore()
        { return _score; }

        public UInt16 GetHP()
        { return _hp; }

        private readonly string _username;
        private readonly EndPoint _endpoint;
        private readonly Socket _usedSocket;
        private readonly Type _type;
        private readonly UInt16 _score;
        private readonly UInt16 _hp;
    }

    private NetPlayer remote;
    private NetPlayer local;

    private void Awake()
    { DontDestroyOnLoad(transform.gameObject); }

    public void AddRemote(string username, EndPoint ep, NetPlayer.Type type, Socket socket)
    { remote = new(username, ep, type, socket); }

    public void SetLocal(string username, NetPlayer.Type type)
    { local = new(username, null, type, null); }

    public NetPlayer GetRemote()
    { return remote; }

    public NetPlayer GetLocal()
    { return local; }

    public void ClearRemote()
    { remote = new("", null, 0, null); }
}