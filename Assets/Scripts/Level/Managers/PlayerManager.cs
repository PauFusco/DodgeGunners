using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;

    private Vector3 netPos;

    private bool localIsHost;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // This makes the distinction between p1 and p2
        if (gameManager.GetEnemy().GetPlayerType() == GameManager.Player.Type.REMOTE)
        {
            localIsHost = true;

            local = hostObj.GetComponent<PlayerBehaviour>();
            remote = remoteObj.GetComponent<PlayerBehaviour>();

            netPos = remote.transform.position;
        }
        else if (gameManager.GetEnemy().GetPlayerType() == GameManager.Player.Type.HOST)
        {
            localIsHost = false;

            remote = hostObj.GetComponent<PlayerBehaviour>();
            local = remoteObj.GetComponent<PlayerBehaviour>();

            netPos = local.transform.position;
        }

        Thread receiveNetMovement = new(ReceiveNetMovement);
        receiveNetMovement.Start();
    }

    private void FixedUpdate()
    {
        CheckKeyMovement(local);
        SendNetMovement(local);
        remote.SetPosition(netPos);
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        if (Input.GetKey(KeyCode.A)) localPlayerToMove.MoveLeft();
        if (Input.GetKey(KeyCode.D)) localPlayerToMove.MoveRight();
        if (Input.GetKey(KeyCode.S)) localPlayerToMove.MoveDown();
        if (Input.GetKey(KeyCode.W)) localPlayerToMove.MoveUp();
    }

    private void SendNetMovement(PlayerBehaviour localPlayerToSend)
    {
        byte[] position = Encoding.ASCII.GetBytes(localPlayerToSend.transform.position.ToString());

        Socket socket = gameManager.GetEnemy().GetSocket();

        if (localIsHost) socket.SendTo(position, gameManager.GetEnemy().GetEndPoint());
        else socket.Send(position);
    }

    private void ReceiveNetMovement()
    {
        Socket socket = gameManager.GetEnemy().GetSocket();

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        byte[] data;
        int recv;

        // Assign netPos with recieved position
        while (true)
        {
            data = new byte[1024];
            
            if (localIsHost) recv = socket.Receive(data);
            else recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            netPos = StringToVector(Encoding.ASCII.GetString(data, 0, recv));
        }
    }

    private Vector3 StringToVector(string str)
    {
        string[] temp = str[1..^1].Split(',');

        return new Vector3(
            float.Parse(temp[0], CultureInfo.InvariantCulture), 
            float.Parse(temp[1], CultureInfo.InvariantCulture), 
            float.Parse(temp[2], CultureInfo.InvariantCulture));
    }
}