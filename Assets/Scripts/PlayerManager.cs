using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;

    private Vector3 netPos;

    private Socket socket;
    private bool localIsHost;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        local = hostObj.GetComponent<PlayerBehaviour>();
        remote = remoteObj.GetComponent<PlayerBehaviour>();

        // This makes the distinction between p1 and p2
        if (gameManager.GetEnemy().GetPlayerType() == GameManager.Player.Type.REMOTE)
        {
            localIsHost = true;
            netPos = remote.transform.position;
            HostSocketSetup();
        }
        else if (gameManager.GetEnemy().GetPlayerType() == GameManager.Player.Type.HOST)
        {
            localIsHost = false;
            netPos = local.transform.position;
            RemoteSocketSetup();
        }

        Thread receiveNetMovement = new(ReceiveNetMovement);
        receiveNetMovement.Start();
    }

    private void FixedUpdate()
    {
        if (localIsHost)
        {
            CheckKeyMovement(local);
            SendNetMovement(local);
            remote.SetPosition(netPos);
        }
        else
        {
            CheckKeyMovement(remote);
            SendNetMovement(remote);
            local.SetPosition(netPos);
        }
    }

    private void HostSocketSetup()
    {
        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);
    }

    private void RemoteSocketSetup()
    {
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Connect(gameManager.GetEnemy().GetEndPoint());
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
        byte[] position = new byte[1024];
        position = Encoding.ASCII.GetBytes(localPlayerToSend.transform.position.ToString());

        if (localIsHost) socket.SendTo(position, gameManager.GetEnemy().GetEndPoint());
        else socket.Send(position);
    }

    private void ReceiveNetMovement()
    {
        // Assign netPos with recieved position
        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (true)
        {
            byte[] data = new byte[1024];
            int recv;
            if (localIsHost) recv = socket.Receive(data);
            else recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            netPos = StringToVector(Encoding.ASCII.GetString(data, 0, recv));
        }
    }

    private Vector3 StringToVector(string str)
    {
        string[] temp = str[1..^1].Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);

        return new Vector3(x, y, z);
    }
}