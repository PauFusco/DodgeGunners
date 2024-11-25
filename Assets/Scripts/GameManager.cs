using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public class NETFLAGS
    {
        public NETFLAGS()
        {
            player = false;
            projectiles = false;
            score = false;
            gameTime = false;
        }

        public bool player;
        public bool projectiles;
        public bool score;
        public bool gameTime;
    }

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

        public UInt16 GetScore()
        { return _score; }

        private readonly string _username;
        private readonly EndPoint _endpoint;
        private readonly Socket _usedSocket;
        private readonly Type _type;
        private readonly UInt16 _score;
    }

    private Player remote;
    private Player local;

    private NETFLAGS _flags;

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