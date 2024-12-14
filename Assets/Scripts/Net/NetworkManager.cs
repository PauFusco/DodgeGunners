using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerManagerObj, projectileControllerObj;

    private PlayerManager playerManager;
    private ProjectileController projectileController;
    private GameManager gameManager;

    private enum PacketType
    {
        PLAYER,
        PROJECTILE,
        DEFAULT
    }

    public class PlayerPacket
    {
        public PlayerPacket(GameManager.NetPlayer playerToSend, PlayerBehaviour playerBHToSend)
        {
            _type = PacketType.PLAYER;

            MemoryStream playerMStream = new();
            BinaryWriter playerBWriter = new(playerMStream);

            playerBWriter.Write((int)_type);

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

        public PlayerPacket(byte[] data, GameManager.NetPlayer playerToModify, PlayerBehaviour playerBHToModify)
        {
            MemoryStream playerMStream = new(data);
            BinaryReader playerBReader = new(playerMStream);

            _type = (PacketType)playerBReader.ReadInt32();

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

        private readonly byte[] _data;
        private Vector3 _position = new();
        private Quaternion _rotation = new();
        private PacketType _type;

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
            public MockProjectile(Vector3 spawnPos, float lifeTime, float spawnTime)
            {
                _spawnpos = spawnPos;
                _spawntime = spawnTime;
                _lifetime = spawnTime;
            }

            public Vector3 GetSpawnPos()
            { return _spawnpos; }

            public float GetSpawnTime()
            { return _spawntime; }

            public float GetLifetime()
            { return _lifetime; }

            private Vector3 _spawnpos;
            private float _spawntime, _lifetime;
        }

        public ProjectilesPacket(List<ProjectileController.Projectile> projectileList)
        {
            _type = PacketType.PROJECTILE;

            MemoryStream projectileMStream = new();
            BinaryWriter projectileBWriter = new(projectileMStream);

            projectileBWriter.Write((int)_type);

            projectileBWriter.Write(projectileList.Count);

            foreach (ProjectileController.Projectile proj in projectileList)
            {
                projectileBWriter.Write(proj.GetSpawnPosition().x);
                projectileBWriter.Write(proj.GetSpawnPosition().y);
                projectileBWriter.Write(proj.GetSpawnPosition().z);

                projectileBWriter.Write(proj.GetSpawnTime());

                projectileBWriter.Write(Time.time);
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

                Vector3 tempSpawn = new(tempx, tempy, tempz);

                float spawntime = projectileBReader.ReadSingle();

                float lifetime = projectileBReader.ReadSingle();

                MockProjectile temp = new(tempSpawn, spawntime, lifetime);
                _netprojectiles.Add(temp);
            }
        }

        private byte[] _data;
        private readonly List<MockProjectile> _netprojectiles;
        private readonly PacketType _type;

        public List<MockProjectile> GetNetProjectiles()
        { return _netprojectiles; }
    }

    private void Start()
    {
        playerManager = playerManagerObj.GetComponent<PlayerManager>();
        projectileController = projectileControllerObj.GetComponent<ProjectileController>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Thread receiveNetMovement = new(RecieveNetInfo);
        receiveNetMovement.Start();
    }

    public void SendNetInfo(PlayerBehaviour localPlayerToSend)
    {
        PlayerPacket localPacket = new(gameManager.GetLocal(), localPlayerToSend);

        Socket socket = gameManager.GetRemote().GetSocket();

        if (playerManager.GetLocalIsHost()) socket.SendTo(localPacket.GetBuffer(), gameManager.GetRemote().GetEndPoint());
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

            MemoryStream packet = new(data);
            BinaryReader BReader = new(packet);

            PacketType ptype = (PacketType)BReader.ReadInt32();

            switch (ptype)
            {
                case PacketType.PLAYER:
                    PlayerPacket PlPacket = new(data, gameManager.GetRemote(), playerManager.GetRemote());
                    playerManager.SetNetPosition(PlPacket.GetPosition());
                    break;

                case PacketType.PROJECTILE:
                    ProjectilesPacket PrPacket = new(data);
                    ModifyProjectilesWithNetInfo(PrPacket.GetNetProjectiles());
                    break;
            }
        }
    }

    private void ModifyProjectilesWithNetInfo(List<ProjectilesPacket.MockProjectile> mockProjectiles)
    {
        projectileController.GetRemoteProjectiles().Clear();

        foreach (ProjectilesPacket.MockProjectile p in mockProjectiles)
        {
            projectileController.RemoteSpawnProjectile(p.GetSpawnPos(), p.GetLifetime());
        }
    }
}