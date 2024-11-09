using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject joinObj, usernameObj, hostIPObj, gameManagerObj;

    private Button join;
    private TMP_InputField usernameInput, hostIPInput;
    private GameManager gameManager;

    private IPEndPoint hostIPEP;
    private Socket socket;

    private void Start()
    {
        join = joinObj.GetComponent<Button>();
        usernameInput = usernameObj.GetComponent<TMP_InputField>();
        hostIPInput = hostIPObj.GetComponent<TMP_InputField>();

        join.onClick.AddListener(LobbyJoin);
    }

    private void Update()
    {
        // if start game -> load scene
        // 
    }

    private void LobbyJoin()
    {
        hostIPEP = new(IPAddress.Parse(hostIPInput.text), 9050);
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(hostIPEP);

        Thread sendUsrnm = new(SendUsername);
        sendUsrnm.Start();
    }

    private void SendUsername()
    {
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

            gameManager.AddEnemy(Encoding.ASCII.GetString(data, 0, recv), hostIPEP);

            break;
        }
    }
}