using System.Net.Sockets;
using System.Net;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateObj, StartObj, LogObj, UsernameInputFieldObj, GameManagerObj;

    private Button CreateButton, StartButton;
    private TextMeshProUGUI Log;
    private TMP_InputField usernameInput;
    private GameManager gameManager;

    Socket socket;

    private void Start()
    {
        CreateButton = CreateObj.GetComponent<Button>();
        StartButton = StartObj.GetComponent<Button>();
        Log = LogObj.GetComponent<TextMeshProUGUI>();
        usernameInput = UsernameInputFieldObj.GetComponent<TMP_InputField>();
        gameManager = GameManagerObj.GetComponent<GameManager>();

        CreateButton.onClick.AddListener(LobbyCreate);
        StartButton.onClick.AddListener(LobbyStart);
    }

    private void LobbyCreate()
    {
        IPEndPoint ipep = new(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        gameManager.ClearPlayerList();
        gameManager.AddNewPlayer(usernameInput.text, ipep);

        CreatePrintLog("Lobby Created");

        Thread newConnectionCheck = new Thread(CheckNewPlayers);
        newConnectionCheck.Start();
    }

    private void LobbyStart()
    {

    }

    private void CheckNewPlayers()
    {
        int recv;
        byte[] data = new byte[1024];

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        while (gameManager.PlayerCount() < 2)
        {
            recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            string message = Encoding.ASCII.GetString(data, 0, recv);

            gameManager.AddNewPlayer(message, remote);
        }
    }

    public void CreatePrintLog(string text)
    {
        Log.text = text;
    }
}