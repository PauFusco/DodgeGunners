using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public class Projectile
    {
        private float baseLifeTimeLimit, baseSpeed, _lifetimelimit, _speed, _spawntime;

        private Vector3 _spawnposition;

        private GameObject projectileObj;

        public Projectile(Vector3 spawnPos)
        {
            _lifetimelimit = baseLifeTimeLimit;
            _speed = baseSpeed;

            _spawntime = Time.time;

            _spawnposition = spawnPos;

            // Instanciate projectile
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

            // Instanciate with newPos
        }

        public GameObject GetGameObject()
        { return projectileObj; }

        public float GetLifetimeLimit()
        { return _lifetimelimit; }

        public float GetSpawnTime()
        { return _spawntime; }

        public float GetSpeed()
        { return _speed; }

        public Vector3 GetSpawnPosition()
        { return _spawnposition; }
    }

    private List<Projectile> localProjectiles = new();
    private List<Projectile> remoteProjectiles = new();

    private void Update()
    {
        foreach (Projectile proj in localProjectiles)
        {
            float currentLifetime = Time.time - proj.GetSpawnTime();
            if (currentLifetime <= 0)
            {
                localProjectiles.Remove(proj);
                continue;
            }

            Vector3 newPos = proj.GetGameObject().transform.position;
            newPos.z += proj.GetSpawnPosition().z <= 0 ? proj.GetSpeed() * currentLifetime : -proj.GetSpeed() * currentLifetime;

            proj.GetGameObject().transform.position = newPos;
        }

        foreach (Projectile proj in remoteProjectiles)
        {
            float currentLifetime = Time.time - proj.GetSpawnTime();
            if (currentLifetime <= 0)
            {
                localProjectiles.Remove(proj);
                continue;
            }

            Vector3 newPos = proj.GetGameObject().transform.position;
            newPos.z += proj.GetSpawnPosition().z <= 0 ? proj.GetSpeed() * currentLifetime : -proj.GetSpeed() * currentLifetime;

            proj.GetGameObject().transform.position = newPos;
        }
    }

    public void LocalSpawnProjectile(Vector3 pos)
    {
        Projectile projectile = new(pos);
        localProjectiles.Add(projectile);
    }

    public void RemoteSpawnProjectile(Vector3 OGSpawnPos, float currentLifeTime)
    {
        Projectile projectile = new(OGSpawnPos, currentLifeTime);
        remoteProjectiles.Add(projectile);
    }

    public List<Projectile> GetLocalProjectiles()
    { return localProjectiles; }

    public List<Projectile> GetRemoteProjectiles()
    { return remoteProjectiles; }
}