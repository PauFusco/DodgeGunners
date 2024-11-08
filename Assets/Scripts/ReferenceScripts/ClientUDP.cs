using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    private Socket socket;
    public GameObject UItextObj;
    private TextMeshProUGUI UItext;
    private string clientText;

    public TMP_InputField inputID;
    public TMP_InputField userName;
    public TMP_InputField chatInput;

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;

        if (Input.GetKeyUp(KeyCode.Return)) { Chat(); }
    }

    public void StartClient()
    {
        Task.Run(() => Send("127.0.0.1"));
    }

    public void StartClient(TMP_InputField inputID)
    {
        Task.Run(() => Send(inputID.text));
    }

    async void Chat()
    {
        clientText = clientText += "\n" + chatInput.text;
        await Task.Run(() => SendMessage(inputID.text, chatInput.text));
        chatInput.text = string.Empty;
    }

    void Send(string ipString)
    {
        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ipString), 9050);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            byte[] data = Encoding.ASCII.GetBytes(userName.text + " wants to connect");
            socket.SendTo(data, ipep);

            Task.Run(() => Receive(ipString));
        }
        catch (SocketException ex)
        {
            Debug.LogError("Socket connection failed: " + ex.Message);
        }
    }
    void SendMessage(string ipString, string chatInput)
    {
        try
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ipString), 9050);

            byte[] data = Encoding.ASCII.GetBytes(userName.text + ": " + chatInput);
            socket.SendTo(data, ipep);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SendMessage error: " + ex.Message);
        }
    }

    void Receive(string ipString)
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ipString), 9050);
        EndPoint remote = (EndPoint)(sender);

        while (true)
        {
            try
            {
                byte[] data = new byte[1024];
                int recv = socket.ReceiveFrom(data, ref remote);

                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                clientText = $"Message received from {remote}: {receivedMessage}";
            }
            catch (SocketException ex)
            {
                Debug.LogError("Receive error: " + ex.Message);
                break;
            }
        }
    }
}