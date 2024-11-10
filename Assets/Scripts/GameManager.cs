using System.Collections;
using System.Collections.Generic;
using System.Net;
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

        public Player(string username, EndPoint endpoint, Type type)
        {
            _username = username;
            _endpoint = endpoint;
            _type = type;
        }

        public string GetUsrnm()
        { return _username; }

        public EndPoint GetEndPoint()
        { return _endpoint; }

        private readonly string _username;
        private readonly EndPoint _endpoint;
        private readonly Type _type;
    }

    private Player enemy;

    private void Awake()
    { DontDestroyOnLoad(transform.gameObject); }

    public void AddEnemy(string username, EndPoint ep, Player.Type type)
    { enemy = new(username, ep, type); }

    public Player GetEnemy()
    { return enemy; }

    public void ClearEnemy()
    { enemy = new("", null, 0); }
}