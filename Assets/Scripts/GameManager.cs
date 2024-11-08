using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private struct Player
    {
        private string _username;
        private EndPoint _endpoint;

        public Player(string username, EndPoint endpoint)
        {
            _username = username;
            _endpoint = endpoint;
        }
    }

    private List<Player> PlayerList;

    public void AddNewPlayer(string username, EndPoint ep)
    {
        PlayerList.Add(new(username, ep));
    }
}