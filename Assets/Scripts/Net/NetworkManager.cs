using System.Globalization;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.IO;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerManagerObj;

    private PlayerManager playerManager;
    private GameManager gameManager;

    public class PlayerPacket
    {
        private PlayerPacket(GameManager.NetPlayer playerToSend, PlayerBehaviour playerBHToSend)
        {
            MemoryStream playerMStream = new();
            BinaryWriter playerBWriter = new(playerMStream);

            // flags to byte
            // frame to byte

            // position to byte
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.x);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.y);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.z);

            // rotation to byte
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.x);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.y);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.z);

            // HP to byte

            // score to byte
            playerBWriter.Write(playerToSend.GetScore());

            // add all to packet->_data
        }

        public void UnPack()
        {
            throw new System.NotImplementedException();
        }

        private byte[] ByteEulerAngles(Quaternion quatRotation)
        {
            byte[] xEul = Encoding.ASCII.GetBytes(playerToSend.transform.rotation.eulerAngles.x.ToString() + ",");
            byte[] yEul = Encoding.ASCII.GetBytes(playerToSend.transform.rotation.eulerAngles.y.ToString() + ",");
            byte[] zEul = Encoding.ASCII.GetBytes(playerToSend.transform.rotation.eulerAngles.z.ToString() + ";");

            return xEul + yEul + zEul;
        }

        public byte[] _data;
    }

    private void Start()
    {
        playerManager = playerManagerObj.GetComponent<PlayerManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Thread receiveNetMovement = new(ReceiveNetMovement);
        receiveNetMovement.Start();
    }

    public void SendNetMovement(PlayerBehaviour localPlayerToSend)
    {
        byte[] position = Encoding.ASCII.GetBytes(localPlayerToSend.transform.position.ToString());

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(position, gameManager.GetRemote().GetEndPoint());
        else socket.Send(position);
    }

    public void ReceiveNetMovement()
    {
        Socket socket = gameManager.GetRemote().GetSocket();

        IPEndPoint sender = new(IPAddress.Any, 0);
        EndPoint remote = sender;

        byte[] data;
        int recv;

        while (true)
        {
            data = new byte[1024];

            if (playerManager.GetLocalIsHost()) recv = socket.Receive(data);
            else recv = socket.ReceiveFrom(data, ref remote);

            if (recv == 0) continue;

            playerManager.SetNetPosition(StringToVector(Encoding.ASCII.GetString(data, 0, recv)));
        }
    }

    private Vector3 StringToVector(string str)
    {
        string[] temp = str[1..^1].Split(',');

        return new Vector3(
            float.Parse(temp[0], CultureInfo.InvariantCulture),
            float.Parse(temp[1], CultureInfo.InvariantCulture),
            float.Parse(temp[2], CultureInfo.InvariantCulture));
    }
}