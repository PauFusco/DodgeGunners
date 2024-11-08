using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{
    private Socket socket;

    public GameObject UItextObj;
    private TextMeshProUGUI UItext;
    private string serverText;

    private readonly List<EndPoint> clients = new();

    private void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UItext.text = serverText;
    }

    public void startServer()
    {
        serverText = "Starting UDP Server...";

        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        Thread newConnection = new(CheckClients);
        newConnection.Start();
    }

    private void CheckClients()
    {
        int recv;
        byte[] data = new byte[1024];

        serverText = serverText + "\n" + "Waiting for new Client...";

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (true)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            string message = Encoding.ASCII.GetString(data, 0, recv);

            CheckUpdateClientList(remote);

            serverText += "\n" + remote.ToString() + ": ";
            serverText += message;

            foreach (EndPoint client in clients)
            {
                string massage = remote.ToString() + ": " + message;

                Thread SendMessage = new(() => Send(massage, client));
                SendMessage.Start();
            }
        }
    }

    private void CheckUpdateClientList(EndPoint remote)
    {
        if (!clients.Contains(remote))
        {
            clients.Add(remote);

            Thread validate = new(() => ValidateConnection(remote));
            validate.Start();
        }
    }

    private void ValidateConnection(EndPoint remote)
    {
        string Ping = "Connection Successful";

        byte[] Ping_Encoded = Encoding.ASCII.GetBytes(Ping);

        socket.SendTo(Ping_Encoded, remote);
    }

    private void Send(string massage, EndPoint remote)
    {
        byte[] data = Encoding.ASCII.GetBytes(massage);

        socket.SendTo(data, remote);
    }
}