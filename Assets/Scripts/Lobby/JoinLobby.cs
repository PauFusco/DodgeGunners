using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JoinLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject joinObj, usernameObj, hostIPObj, gameManagerObj, logObj;

    private Button join;
    private TMP_InputField usernameInput, hostIPInput;
    private GameManager gameManager;

    private IPEndPoint hostIPEP;
    private Socket socket;

    private bool startGame;

    private TextMeshProUGUI log;
    private string debugText;

    private IPAddress serverIP;

    private void Start()
    {
        // Put server IP here
        serverIP = IPAddress.Parse("192.168.1.131");

        startGame = false;

        join = joinObj.GetComponent<Button>();
        usernameInput = usernameObj.GetComponent<TMP_InputField>();
        hostIPInput = hostIPObj.GetComponent<TMP_InputField>();

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
        hostIPEP = new(IPAddress.Parse(hostIPInput.text), 9050);
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(hostIPEP);
        debugText = "Waiting to join...";

        gameManager.SetLocal(usernameInput.text, GameManager.NetPlayer.Type.REMOTE);

        byte[] username = new byte[1024];
        username = Encoding.ASCII.GetBytes(usernameInput.text);

        socket.SendTo(username, hostIPEP);

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

            gameManager.AddRemote(Encoding.ASCII.GetString(data, 0, recv), hostIPEP, GameManager.NetPlayer.Type.HOST, socket);

            debugText = "You have joined " + Encoding.ASCII.GetString(data, 0, recv) + "'s lobby!";

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