using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerUDP : MonoBehaviour
{
    Socket socket;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartServer()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        Thread newConnection = new Thread(Receive);
        newConnection.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Receive()
    {
        int recv;
        byte[] data = new byte[1024];

        //serverText = serverText + "\n" + "Waiting for new Client...";

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)(sender);

        while (true)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0)
                break;
            else
            {
                //serverText = serverText + "\n" + "Message received from:" + remote.ToString();
                //serverText = serverText + "\n" + Encoding.ASCII.GetString(data, 0, recv);
            }

            Thread sendThread = new Thread(() => Send(remote));
            sendThread.Start();
        }
    }

    void Send(EndPoint Remote)
    {
        byte[] data = Encoding.ASCII.GetBytes("Connected to server " /*+ serverName.text*/);
        socket.SendTo(data, Remote);
    }
}
