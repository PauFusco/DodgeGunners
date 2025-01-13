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
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button join;
    [SerializeField] private TMP_InputField usernameInput, hostIPInput;
    [SerializeField] private TextMeshProUGUI log;

    private IPEndPoint hostIPEP;
    private Socket socket;

    private string debugText = "";
    private bool startGame = false;

    private void Start()
    {
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
        username = Encoding.UTF8.GetBytes(usernameInput.text);

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

            gameManager.AddRemote(Encoding.UTF8.GetString(data, 0, recv), hostIPEP, GameManager.NetPlayer.Type.HOST, socket);

            debugText = "You have joined " + Encoding.UTF8.GetString(data, 0, recv) + "'s lobby!";

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