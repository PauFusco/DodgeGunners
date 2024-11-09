using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private struct Player
    {
        readonly string _username;
        readonly EndPoint _endpoint;

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

    private List<Player> PlayerList = new();

    public void AddNewPlayer(string username, EndPoint ep)
    {
        Player newPlayer = new(username, ep);
        PlayerList.Add(newPlayer);
    }

    public void ClearPlayerList()
    {
        PlayerList.Clear();
    }

    public int PlayerCount()
    {
        return PlayerList.Count;
    }
}