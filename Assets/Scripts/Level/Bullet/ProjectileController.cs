using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private Object projectileObj;

    public class Projectile
    {
        private readonly float baseLifeTimeLimit = 1.0f, baseSpeed = 1.0f, _lifetimelimit = 1.0f, _speed = 1.0f, _spawntime = 1.0f;

        private Vector3 _spawnposition;
        private Vector3 _spawntocurrentposition;

        public GameObject projectileObj;

        public Projectile(Vector3 spawnPos)
        {
            _lifetimelimit = baseLifeTimeLimit;
            _speed = baseSpeed;

            _spawntime = Time.time;

            _spawnposition = spawnPos;
            _spawntocurrentposition = spawnPos;
        }

        public Projectile(Vector3 spawnPos, float spawnTime)
        {
            float currentLifeTime = Time.time - spawnTime;

            if (currentLifeTime <= 0) return;

            _lifetimelimit = baseLifeTimeLimit;
            _speed = baseSpeed;
            _spawntime = spawnTime;

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

    private readonly List<Projectile> localProjectiles = new();
    private readonly List<Projectile> remoteProjectiles = new();

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
            newPos.z += proj.GetSpawnPosition().z <= 0 ? proj.GetSpeed() * currentLifetime : -proj.GetSpeed() * currentLifetime;
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
        Projectile projectile = new(pos);
        projectile.projectileObj =
            (GameObject)Instantiate(projectileObj, projectile.GetSpawnPosition(), Quaternion.identity);
        localProjectiles.Add(projectile);
    }

    public void RemoteSpawnProjectile(Vector3 OGSpawnPos, float currentLifeTime)
    {
        Projectile projectile = new(OGSpawnPos, currentLifeTime);
        projectile.projectileObj =
            (GameObject)Instantiate(projectileObj, projectile.GetSpawnToCurrentPosition(), Quaternion.identity);
        remoteProjectiles.Add(projectile);
    }

    public List<Projectile> GetLocalProjectiles()
    { return localProjectiles; }

    public List<Projectile> GetRemoteProjectiles()
    { return remoteProjectiles; }
}