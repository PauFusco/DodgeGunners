using System.Net.Sockets;
using System.Net;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using Unity.VisualScripting;

internal enum LobbyMessageType
{
    CREATE,
    JOIN,
    DEFAULT
}

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject createObj, startObj, usernameInputFieldObj, hostObj, logObj, gameManagerObj;

    private Button createButton, startButton;
    private TextMeshProUGUI log;
    private TMP_InputField usernameInput;
    private TextMeshProUGUI hostCode;
    private GameManager gameManager;

    private Socket socket;

    private int roomCode;
    private string debugText;
    private string roomText;
    private bool startGame = false;

    private IPAddress serverIP;

    private void Start()
    {
        // Put server IP here
        serverIP = IPAddress.Parse("192.168.56.1");
        roomCode = -1;
        roomText = "";

        createButton = createObj.GetComponent<Button>();
        startButton = startObj.GetComponent<Button>();
        log = logObj.GetComponent<TextMeshProUGUI>();
        usernameInput = usernameInputFieldObj.GetComponent<TMP_InputField>();
        hostCode = hostObj.GetComponent<TextMeshProUGUI>();
        gameManager = gameManagerObj.GetComponent<GameManager>();

        createButton.onClick.AddListener(LobbyCreate);
        startButton.onClick.AddListener(LobbyStart);
        startButton.interactable = startGame;
    }

    private void Update()
    {
        log.text = debugText;
        hostCode.text = roomText;
        hostCode.text = roomCode.ToString();
        startButton.interactable = startGame;
    }

    private void LobbyCreate()
    {
        IPEndPoint serverIPEP = new(serverIP, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(serverIPEP);

        gameManager.ClearRemote();

        MemoryStream usernameMS = new();
        BinaryWriter BW = new(usernameMS);

        LobbyMessageType type = LobbyMessageType.CREATE;
        BW.Write((int)type);
        BW.Write(usernameInput.text);

        socket.Send(usernameMS.ToArray());

        debugText = "Creating Room...";

        Thread checkRoomCode = new(CheckServerResponseCode);
        checkRoomCode.Start();
    }

    private void CheckServerResponseCode()
    {
        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (roomCode == -1)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            roomCode = UInt16.Parse(Encoding.ASCII.GetString(data));
        }

        Thread newConnectionCheck = new(CheckNewPlayer);
        newConnectionCheck.Start();
    }

    private void CheckNewPlayer()
    {
        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (gameManager.GetRemote().GetIPEndPoint() == null)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            MemoryStream remoteMS = new(data);
            BinaryReader remoteMSBR = new(remoteMS);

            string remoteUsername = remoteMSBR.ReadString();
            IPAddress remoteIP = IPAddress.Parse(remoteMSBR.ReadString());

            IPEndPoint binderIPEP = new(IPAddress.Any, 50000);
            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(binderIPEP);

            IPEndPoint remoteipep = new(remoteIP, 50000);
            gameManager.AddRemote(remoteUsername, remoteipep, GameManager.NetPlayer.Type.REMOTE, socket);

            gameManager.SetLocal(usernameInput.text, GameManager.NetPlayer.Type.HOST);

            startGame = true;
        }
    }

    private void LobbyStart()
    {
        byte[] startGame = Encoding.ASCII.GetBytes("StartGame");
        socket.SendTo(startGame, gameManager.GetRemote().GetIPEndPoint());

        SceneManager.LoadScene(1);
    }

    public void CopyCode()
    {
        if (hostCode != null)
        {
            GUIUtility.systemCopyBuffer = hostCode.text;
            debugText = "Copied to Clipboard!";
        }
        else
            debugText = "No IP found!";
    }
}