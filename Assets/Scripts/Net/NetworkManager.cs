using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.IO;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerManagerObj;

    private PlayerManager playerManager;
    private GameManager gameManager;

    public class PlayerPacket
    {
        public PlayerPacket(GameManager.NetPlayer playerToSend, PlayerBehaviour playerBHToSend)
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
            //playerBWriter.Write(playerToSend.GetScore());

            // add all to packet->_data
            _data = playerMStream.ToArray();
        }

        /// <summary>
        /// Creates a packet using a buffer and modifies the player set in the definition
        /// </summary>
        /// <param name="data"></param>
        /// <param name="playerToModify"></param>
        public PlayerPacket(byte[] data, GameManager.NetPlayer playerToModify, PlayerBehaviour playerBHToModify)
        {
            MemoryStream playerMStream = new(data);
            BinaryReader playerBReader = new(playerMStream);

            // frame
            // flags

            //position
            Vector3 newPosition = new()
            {
                x = playerBReader.ReadSingle(),
                y = playerBReader.ReadSingle(),
                z = playerBReader.ReadSingle()
            };
            _position = newPosition;

            // rotation
            Vector3 newERotation = new()
            {
                x = playerBReader.ReadSingle(),
                y = playerBReader.ReadSingle(),
                z = playerBReader.ReadSingle()
            };
            _rotation = Quaternion.Euler(newERotation);

            // HP
            // score
        }

        public byte[] GetBuffer()
        { return _data; }

        public Vector3 GetPosition()
        { return _position; }

        public Quaternion GetRotation()
        { return _rotation; }

        private readonly byte[] _data;
        private Vector3 _position = new();
        private Quaternion _rotation = new();
    }

    private void Start()
    {
        playerManager = playerManagerObj.GetComponent<PlayerManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Thread receiveNetMovement = new(RecieveNetInfo);
        receiveNetMovement.Start();
    }

    public void SendNetInfo(PlayerBehaviour localPlayerToSend)
    {
        PlayerPacket localPacket = new(gameManager.GetLocal(), localPlayerToSend);

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(localPacket.GetBuffer(), gameManager.GetRemote().GetIPEndPoint());
        else socket.Send(localPacket.GetBuffer());
    }

    public void RecieveNetInfo()
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

            PlayerPacket packet = new(data, gameManager.GetRemote(), playerManager.GetRemote());
            playerManager.SetNetPosition(packet.GetPosition());
        }
    }
}