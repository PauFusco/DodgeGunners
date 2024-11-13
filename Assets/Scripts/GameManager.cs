using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly struct Player
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

    private Player enemy;
    private Player local;

    private void Awake()
    { DontDestroyOnLoad(transform.gameObject); }

    public void AddEnemy(string username, EndPoint ep, Player.Type type, Socket socket)
    { enemy = new(username, ep, type, socket); }

    public void SetLocal(string username, Player.Type type)
    { local = new(username, null, type, null); }

    public Player GetEnemy()
    { return enemy; }

    public Player GetLocal()
    { return local; }

    public void ClearEnemy()
    { enemy = new("", null, 0, null); }
}