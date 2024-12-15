using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Object projectilePF;

    public class Projectile
    {
        private readonly float _lifetimelimit, _speed, _spawntime;

        private Vector3 _spawnposition;
        private Vector3 _spawntocurrentposition;

        public GameObject projectileObj;

        public Projectile(Vector3 spawnPos)
        {
            _lifetimelimit = 1.0f;
            _speed = 0.5f;

            _spawntime = Time.time;

            _spawnposition = spawnPos;
            _spawntocurrentposition = spawnPos;
        }

        public Projectile(Vector3 spawnPos, float spawnTime)
        {
            _lifetimelimit = 1.0f;
            _speed = 0.5f;
            _spawntime = spawnTime;

            float currentLifeTime = Time.time - spawnTime;

            if (currentLifeTime >= _lifetimelimit) return;

            Vector3 newPos = spawnPos;
            newPos.z += spawnPos.z <= 0 ? _speed * currentLifeTime : -_speed * currentLifeTime;
            _spawntocurrentposition = newPos;
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
    }

    [SerializeField]
    private GameObject networkManagerObj;

    private NetworkManager networkManager;

    private readonly List<Projectile> localProjectiles = new();
    private readonly List<Projectile> remoteProjectiles = new();

    private void Start()
    {
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
    }

    private void Update()
    {
        for (int i = localProjectiles.Count - 1; i >= 0; i--)
        {
            var proj = localProjectiles[i];
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

        for (int i = remoteProjectiles.Count - 1; i >= 0; i--)
        {
            var proj = remoteProjectiles[i];
            float currentLifetime = Time.time - proj.GetSpawnTime();
            if (currentLifetime >= proj.GetLifetimeLimit())
            {
                Destroy(proj.projectileObj);
                remoteProjectiles.RemoveAt(i);
                continue;
            }

            Vector3 newPos = proj.projectileObj.transform.position;
            newPos.z += proj.GetSpawnPosition().z <= 0 ? proj.GetSpeed() * currentLifetime : -proj.GetSpeed() * currentLifetime;
            proj.projectileObj.transform.position = newPos;
        }
    }

    public void LocalSpawnProjectile(Vector3 pos)
    {
        Projectile proj = new(pos);
        proj.projectileObj =
            (GameObject)Instantiate(projectilePF, proj.GetSpawnPosition(), Quaternion.identity);

        localProjectiles.Add(proj);
    }
}