using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    private enum PacketType
    {
        PLAYER,
        PROJECTILE,
        GAME_DATA,
        DEFAULT
    }

    public class PlayerPacket
    {
        public PlayerPacket(PlayerBehaviour playerBHToSend)
        {
            _type = PacketType.PLAYER;

            MemoryStream playerMStream = new();
            BinaryWriter playerBWriter = new(playerMStream);

            playerBWriter.Write((int)_type);

            // position to byte
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.x);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.y);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().position.z);

            // rotation to byte
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.x);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.y);
            playerBWriter.Write(playerBHToSend.GetLocalTransform().rotation.eulerAngles.z);

            // HP to byte
            playerBWriter.Write(playerBHToSend.GetHealth());

            // score to byte
            playerBWriter.Write(playerBHToSend.GetScore());

            // add all to packet->_data
            _data = playerMStream.ToArray();
        }

        public PlayerPacket(byte[] data, PlayerBehaviour playerBHToModify)
        {
            MemoryStream playerMStream = new(data);
            BinaryReader playerBReader = new(playerMStream);

            _type = (PacketType)playerBReader.ReadInt32();

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
        }

        private readonly byte[] _data;
        private Vector3 _position = new();
        private Quaternion _rotation = new();

        private readonly PacketType _type;

        public byte[] GetBuffer()
        { return _data; }

        public Vector3 GetPosition()
        { return _position; }

        public Quaternion GetRotation()
        { return _rotation; }
    }

    public class ProjectilesPacket
    {
        public class MockProjectile
        {
            public MockProjectile(Vector3 position, Quaternion rotation)
            {
                _position = position;
                _rotation = rotation;
            }

            public Vector3 GetPosition()
            { return _position; }

            public Quaternion GetRotation()
            { return _rotation; }

            private Vector3 _position;
            private Quaternion _rotation;
        }

        public ProjectilesPacket(List<ProjectileController.LocalProjectile> projectileList)
        {
            _type = PacketType.PROJECTILE;

            MemoryStream projectileMStream = new();
            BinaryWriter projectileBWriter = new(projectileMStream);

            projectileBWriter.Write((int)_type);

            projectileBWriter.Write(projectileList.Count);

            foreach (ProjectileController.LocalProjectile proj in projectileList)
            {
                projectileBWriter.Write(proj.projectileObj.transform.position.x);
                projectileBWriter.Write(proj.projectileObj.transform.position.y);
                projectileBWriter.Write(proj.projectileObj.transform.position.z);

                projectileBWriter.Write(proj.projectileObj.transform.rotation.w);
                projectileBWriter.Write(proj.projectileObj.transform.rotation.x);
                projectileBWriter.Write(proj.projectileObj.transform.rotation.y);
                projectileBWriter.Write(proj.projectileObj.transform.rotation.z);
            }

            _data = projectileMStream.ToArray();
        }

        public ProjectilesPacket(byte[] data)
        {
            MemoryStream projectileMStream = new(data);
            BinaryReader projectileBReader = new(projectileMStream);

            _type = (PacketType)projectileBReader.ReadInt32();

            int listSize = projectileBReader.ReadInt32();

            for (int i = 0; i < listSize; i++)
            {
                float tempx = projectileBReader.ReadSingle();
                float tempy = projectileBReader.ReadSingle();
                float tempz = projectileBReader.ReadSingle();
                Vector3 tempPos = new(tempx, tempy, tempz);

                float tempw = projectileBReader.ReadSingle();
                tempx = projectileBReader.ReadSingle();
                tempy = projectileBReader.ReadSingle();
                tempz = projectileBReader.ReadSingle();
                Quaternion tempRot = new(tempx, tempy, tempz, tempw);

                MockProjectile temp = new(tempPos, tempRot);
                _netprojectiles.Add(temp);
            }
        }

        private readonly byte[] _data;
        private readonly List<MockProjectile> _netprojectiles = new();
        private readonly PacketType _type;

        public byte[] GetBuffer()
        { return _data; }

        public List<MockProjectile> GetNetProjectiles()
        { return _netprojectiles; }
    }

    public class GameInfoPacket
    {
        public GameInfoPacket(PlayerBehaviour playerBHToSend)
        {
            _type = PacketType.GAME_DATA;

            MemoryStream gameMStream = new();
            BinaryWriter gameBWriter = new(gameMStream);

            gameBWriter.Write((int)_type);

            gameBWriter.Write(playerBHToSend.GetHealth());

            gameBWriter.Write(playerBHToSend.GetScore());

            _data = gameMStream.ToArray();
        }

        public GameInfoPacket(byte[] data, PlayerBehaviour playerBHToModify)
        {
            MemoryStream gameMStream = new(data);
            BinaryReader gameBReader = new(gameMStream);

            _type = (PacketType)gameBReader.ReadInt32();

            _health = gameBReader.ReadSingle();

            _score = gameBReader.ReadInt32();
        }

        private readonly byte[] _data;
        private readonly float _health;
        private readonly int _score;
        private readonly PacketType _type;

        public byte[] GetBuffer()
        { return _data; }

        public float GetHealth()
        { return _health; }

        public int GetScore()
        { return _score; }
    }

    [SerializeField]
    private GameObject playerManagerObj;

    private PlayerManager playerManager;
    private GameManager gameManager;

    private List<ProjectilesPacket.MockProjectile> NetProjectiles = new();

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = playerManagerObj.GetComponent<PlayerManager>();

        Thread receiveNetMovement = new(RecieveNetInfo);
        receiveNetMovement.Start();
    }

    public List<ProjectilesPacket.MockProjectile> GetNetProjectiles()
    { return NetProjectiles; }

    public void SendPlayerNetInfo(PlayerBehaviour localPlayerToSend)
    {
        PlayerPacket localPacket = new(localPlayerToSend);

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(localPacket.GetBuffer(), gameManager.GetRemote().GetEndPoint());
        else socket.Send(localPacket.GetBuffer());
    }

    public void SendProjectilesNetInfo(List<ProjectileController.LocalProjectile> projectiles)
    {
        ProjectilesPacket localPacket = new(projectiles);

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(localPacket.GetBuffer(), gameManager.GetRemote().GetEndPoint());
        else socket.Send(localPacket.GetBuffer());
    }

    public void SendNetGameInfo(PlayerBehaviour localPlayer)
    {
        GameInfoPacket GIPacket = new(localPlayer);

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(GIPacket.GetBuffer(), gameManager.GetRemote().GetEndPoint());
        else socket.Send(GIPacket.GetBuffer());
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

            MemoryStream packet = new(data);
            BinaryReader BReader = new(packet);

            PacketType ptype = (PacketType)BReader.ReadInt32();

            switch (ptype)
            {
                case PacketType.PLAYER:
                    PlayerPacket PlPacket = new(data, playerManager.GetRemote());
                    playerManager.SetNetPosition(PlPacket.GetPosition());
                    break;

                case PacketType.PROJECTILE:
                    ProjectilesPacket PrPacket = new(data);
                    NetProjectiles = PrPacket.GetNetProjectiles();
                    break;

                case PacketType.GAME_DATA:
                    GameInfoPacket GPacket = new(data, playerManager.GetRemote());
                    playerManager.SetNetHealth(GPacket.GetHealth());
                    playerManager.SetNetScore(GPacket.GetScore());
                    break;
            }
        }
    }
}