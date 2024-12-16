using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Object projectilePF;

    public class LocalProjectile
    {
        private readonly float _lifetimelimit, _speed, _spawntime;

        private Vector3 _spawnposition;
        private Vector3 _spawntocurrentposition;

        public GameObject projectileObj;

        public bool IsDestroyed { get; private set; } = false;

        public LocalProjectile(Vector3 spawnPos)
        {
            _lifetimelimit = 1.0f;
            _speed = 0.5f;

            _spawntime = Time.time;

            _spawnposition = spawnPos;
            _spawntocurrentposition = spawnPos;
        }

        public float GetLifetimeLimit()
        { return _lifetimelimit; }

        public float GetSpawnTime()
        { return _spawntime; }

        public float GetSpeed()
        { return _speed; }

        public Vector3 GetSpawnPosition()
        { return _spawnposition; }

        public Vector3 GetSpawnToCurrentPosition()
        { return _spawntocurrentposition; }

        public void MarkAsDestroyed() 
        { IsDestroyed = true; }
    }

    public class RemoteProjectile
    {
        public GameObject _projectileobj;

        public RemoteProjectile(Vector3 position, GameObject projectileObj)
        {
            _projectileobj = projectileObj;
            _projectileobj.transform.position = position;
        }
    }

    [SerializeField]
    private GameObject networkManagerObj;

    private NetworkManager networkManager;

    private readonly List<LocalProjectile> localProjectiles = new();
    private readonly List<RemoteProjectile> remoteProjectiles = new();

    private void Start()
    {
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
    }

    private void Update()
    {
        for (int i = localProjectiles.Count - 1; i >= 0; i--)
        {
            var proj = localProjectiles[i];
            if (proj.projectileObj == null)
            {
                localProjectiles.RemoveAt(i);
                continue;
            }

            float currentLifetime = Time.time - proj.GetSpawnTime();
            if (currentLifetime >= proj.GetLifetimeLimit())
            {
                Destroy(proj.projectileObj);
                localProjectiles.RemoveAt(i);
                continue;
            }

            Vector3 newPos = proj.projectileObj.transform.position;
            newPos.z += proj.GetSpawnPosition().z <= 0 ?
                proj.GetSpeed() * currentLifetime : -proj.GetSpeed() * currentLifetime;

            proj.projectileObj.transform.position = newPos;
        }
    }

    private void FixedUpdate()
    {
        networkManager.SendProjectilesNetInfo(localProjectiles);

        ClearRemoteProjectiles();

        Quaternion rotation = new(0.7071068f, 0, 0, 0.7071068f);

        if (networkManager.GetNetProjectiles().Count > 0)
        {
            foreach (var proj in networkManager.GetNetProjectiles())
            {
                RemoteProjectile temp = new(proj.GetPosition(), (GameObject)Instantiate(projectilePF, proj.GetPosition(), rotation));
                remoteProjectiles.Add(temp);
            }

            networkManager.GetNetProjectiles().Clear();
        }
    }

    private void ClearRemoteProjectiles()
    {
        foreach (var proj in remoteProjectiles)
        {
            Destroy(proj._projectileobj);
        }

        remoteProjectiles.Clear();
    }

    public void LocalSpawnProjectile(Vector3 pos)
    {
        Quaternion rotation = new(0.7071068f, 0, 0, 0.7071068f);

        LocalProjectile proj = new(pos);
        proj.projectileObj = (GameObject)Instantiate(projectilePF, proj.GetSpawnPosition(), rotation);

        localProjectiles.Add(proj);
    }
}