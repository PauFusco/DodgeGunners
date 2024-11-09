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

    string debugText;
    bool startGame = false;

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

        gameManager.ClearEnemy();

        debugText = "Lobby Created";

        Thread newConnectionCheck = new(CheckNewPlayers);
        newConnectionCheck.Start();
    }

    private void LobbyStart()
    {
        if (gameManager.GetEnemy().GetEndPoint() == null) return;

        //Start Game
        //Player Number must be 1 enemy

        Debug.Log("Start Game");
        SceneManager.LoadScene(1);
    }

    private void CheckNewPlayers()
    {
        int recv;
        byte[] data = new byte[1024];

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (gameManager.GetEnemy().GetEndPoint() == null)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            string message = Encoding.ASCII.GetString(data, 0, recv);
            debugText = message + " Just Joined!";

            gameManager.AddEnemy(message, remote);
            startGame = true;
        }
    }

    private void Response()
    {
        byte[] username = new byte[1024];

        username = Encoding.ASCII.GetBytes(usernameInput.text);

        socket.SendTo(username, gameManager.GetEnemy().GetEndPoint());
    }

    public void CreatePrintLog(string text)
    {
        log.text = text;
    }
}