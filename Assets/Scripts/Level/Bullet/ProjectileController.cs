using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private class Projectile
    {
        [SerializeField]
        public float _lifeTime, _speed;

        private int _spawnframe;
        private Vector3 _spawnposition;
    }

    public class NetProjectile
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}