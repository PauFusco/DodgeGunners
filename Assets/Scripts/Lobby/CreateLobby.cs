using System.Net.Sockets;
using System.Net;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject createObj, startObj, logObj, usernameInputFieldObj, gameManagerObj;

    private Button createButton, startButton;
    private TextMeshProUGUI log;
    private TMP_InputField usernameInput;
    private GameManager gameManager;

    private Socket socket;

    private string debugText;
    private bool startGame = false;

    private void Start()
    {
        createButton = createObj.GetComponent<Button>();
        startButton = startObj.GetComponent<Button>();
        log = logObj.GetComponent<TextMeshProUGUI>();
        usernameInput = usernameInputFieldObj.GetComponent<TMP_InputField>();
        gameManager = gameManagerObj.GetComponent<GameManager>();

        createButton.onClick.AddListener(LobbyCreate);
        startButton.onClick.AddListener(LobbyStart);
        startButton.interactable = startGame;
    }

    private void Update()
    {
        log.text = debugText;
        startButton.interactable = startGame;
    }

    private void LobbyCreate()
    {
        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        gameManager.ClearRemote();

        debugText = "Lobby Created";

        Thread newConnectionCheck = new(CheckNewPlayers);
        newConnectionCheck.Start();
    }

    private void LobbyStart()
    {
        byte[] startGame = Encoding.ASCII.GetBytes("StartGame");

        socket.SendTo(startGame, gameManager.GetRemote().GetEndPoint());

        SceneManager.LoadScene(1);
    }

    private void CheckNewPlayers()
    {
        byte[] data = new byte[1024];
        int recv;

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (gameManager.GetRemote().GetEndPoint() == null)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            string message = Encoding.ASCII.GetString(data, 0, recv);
            debugText = message + " Just Joined!";

            gameManager.AddRemote(message, remote, GameManager.Player.Type.REMOTE, socket);

            gameManager.SetLocal(usernameInput.text, GameManager.Player.Type.HOST);

            byte[] username = Encoding.ASCII.GetBytes(usernameInput.text);

            socket.SendTo(username, gameManager.GetRemote().GetEndPoint());

            startGame = true;
        }
    }
}