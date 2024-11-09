using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobby : MonoBehaviour
{
    private GameObject joinObj, usernameObj, hostIPObj;

    private Button join;
    private TMP_InputField usernameInput, hostIPInput;

    private IPEndPoint serverIPEP;
    private Socket socket;

    private void Start()
    {
        join = joinObj.GetComponent<Button>();
        usernameInput = usernameObj.GetComponent<TMP_InputField>();
        hostIPInput = hostIPObj.GetComponent<TMP_InputField>();

        join.onClick.AddListener(LobbyJoin);
    }

    private void LobbyJoin()
    {
        serverIPEP = new(IPAddress.Parse(hostIPInput.text), 9050);
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(serverIPEP);

        Thread sendUsrnm = new(SendUsername);
        sendUsrnm.Start();
    }

    private void SendUsername()
    {
    }
}