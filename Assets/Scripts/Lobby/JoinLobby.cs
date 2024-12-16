using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class JoinLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject joinObj, usernameObj, roomCodeObj, gameManagerObj, logObj;

    private Button join;
    private TMP_InputField usernameInput, roomCodeInput;
    private GameManager gameManager;

    private Socket socket;

    private bool startGame;

    private TextMeshProUGUI log;
    private string debugText;

    private IPAddress serverIP;

    private void Start()
    {
        // Put server IP here
        serverIP = IPAddress.Parse("192.168.56.1");

        startGame = false;

        join = joinObj.GetComponent<Button>();
        usernameInput = usernameObj.GetComponent<TMP_InputField>();
        roomCodeInput = roomCodeObj.GetComponent<TMP_InputField>();

        gameManager = gameManagerObj.GetComponent<GameManager>();

        log = logObj.GetComponent<TextMeshProUGUI>();

        join.onClick.AddListener(LobbyJoin);
    }

    private void Update()
    {
        if (startGame)
        {
            SceneManager.LoadScene(1);
        }
        log.text = debugText;
    }

    private void LobbyJoin()
    {
        IPEndPoint serverIPEP = new(serverIP, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(serverIPEP);

        //debugText = "Waiting to join...";

        gameManager.SetLocal(usernameInput.text, GameManager.NetPlayer.Type.REMOTE);

        MemoryStream roomRequestMS = new();
        BinaryWriter BW = new(roomRequestMS);

        LobbyMessageType type = LobbyMessageType.JOIN;
        BW.Write((int)type);
        BW.Write(usernameInput.text);
        BW.Write(roomCodeInput.text);

        socket.Send(roomRequestMS.ToArray());

        Thread recieveEnemyUsrnm = new(RecieveEnemyUsername);
        recieveEnemyUsrnm.Start();
    }

    private void RecieveEnemyUsername()
    {
        byte[] data;
        int recv;

        while (true)
        {
            data = new byte[1024];
            recv = socket.Receive(data);

            if (recv == 0) continue;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            MemoryStream remoteMS = new(data);
            BinaryReader remoteMSBR = new(remoteMS);

            string remoteUsername = remoteMSBR.ReadString();
            IPAddress remoteIP = IPAddress.Parse(remoteMSBR.ReadString());

            IPEndPoint hostIPEP = new(remoteIP, 50000);
            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(hostIPEP);

            gameManager.AddRemote(remoteUsername, hostIPEP, GameManager.NetPlayer.Type.HOST, socket);

            debugText = "You have joined " + remoteIP.ToString() + "'s lobby!";

            Thread checkGameStart = new(RecieveGameStart);
            checkGameStart.Start();

            break;
        }
    }

    private void RecieveGameStart()
    {
        byte[] data;
        int recv;

        while (true)
        {
            data = new byte[1024];
            recv = socket.Receive(data);

            if (recv == 0) continue;

            if (Encoding.ASCII.GetString(data, 0, recv) == "StartGame")
            {
                startGame = true;
                break;
            }
        }
    }
}