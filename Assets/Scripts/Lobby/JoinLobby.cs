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

    private void Start()
    {
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
            //socket.Close();
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

        byte[] username = new byte[1024];
        username = Encoding.ASCII.GetBytes(usernameInput.text);

        socket.SendTo(username, hostIPEP);

        Thread recieveEnemyUsrnm = new(RecieveEnemyUsername);
        recieveEnemyUsrnm.Start();
    }

    private void RecieveEnemyUsername()
    {
        while (true)
        {
            byte[] data = new byte[1024];
            int recv = socket.Receive(data);

            if (recv == 0) continue;

            gameManager.AddEnemy(Encoding.ASCII.GetString(data, 0, recv), hostIPEP, GameManager.Player.Type.HOST, socket);

            debugText = "You have joined " + Encoding.ASCII.GetString(data, 0, recv) + "'s lobby!";

            Thread checkGameStart = new(RecieveGameStart);
            checkGameStart.Start();

            break;
        }
    }

    private void RecieveGameStart()
    {
        while (true)
        {
            byte[] data = new byte[1024];
            int recv = socket.Receive(data);

            if (recv == 0) continue;

            if (Encoding.ASCII.GetString(data, 0, recv) == "StartGame")
            {
                startGame = true;
                break;
            }
        }
    }
}