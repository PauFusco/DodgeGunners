using System.Collections;
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
            _type = type;
            _usedSocket = socket;
        }

        public string GetUseraname()
        { return _username; }

        public EndPoint GetEndPoint()
        { return _endpoint; }

        public Type GetPlayerType()
        { return _type; }

        public Socket GetSocket()
        { return _usedSocket; }

        private readonly string _username;
        private readonly EndPoint _endpoint;
        private readonly Type _type;
        private readonly Socket _usedSocket;
    }

    private Player enemy;

    private void Awake()
    { DontDestroyOnLoad(transform.gameObject); }

    public void AddEnemy(string username, EndPoint ep, Player.Type type, Socket socket)
    { enemy = new(username, ep, type, socket); }

    public Player GetEnemy()
    { return enemy; }

    public void ClearEnemy()
    { enemy = new("", null, 0, null); }
}