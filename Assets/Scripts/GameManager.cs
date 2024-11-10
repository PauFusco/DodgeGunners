using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly struct Player
    {
        private enum Type
        {
            HOST,
            REMOTE
        }
        
        private readonly string _username;
        private readonly EndPoint _endpoint;

        public Player(string username, EndPoint endpoint)
        {
            _username = username;
            _endpoint = endpoint;
        }

        public string GetUsrnm()
        { return _username; }

        public EndPoint GetEndPoint()
        { return _endpoint; }

    }

    private Player enemy;

    public void AddEnemy(string username, EndPoint ep)
    {
        enemy = new(username, ep);
    }

    public Player GetEnemy()
    {
        return enemy;
    }

    public void ClearEnemy()
    {
        enemy = new("", null);
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}