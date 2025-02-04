using UnityEngine;
using UnityEngine.Pool;

namespace Enemies.Spider
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float spawnRate;
        [SerializeField] private float spawnDuration;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float torqueForce;

        private ObjectPool<GameObject> _projectilePool;
        private float _spawnTimer;
        private float _spawnDurationTimer;
        private bool _isSpawning;
        
        private void Start()
        {
            _spawnTimer = 0f;
            _spawnDurationTimer = 0f;
            _projectilePool =
                new ObjectPool<GameObject>(() => Instantiate(projectilePrefab), HandleOnGet, HandleOnRelease);
        }

        private void HandleOnRelease(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.position = transform.position;
        }

        private void HandleOnGet(GameObject obj)
        {
            obj.SetActive(true);
            obj.transform.position = transform.position;
        }

        private void Update()
        {
            if (_isSpawning)
            {
                _spawnTimer += Time.deltaTime;
                _spawnDurationTimer += Time.deltaTime;
                if (_spawnTimer >= spawnRate)
                {
                    SpawnProjectile();
                    _spawnTimer = 0f;
                }

                if (_spawnDurationTimer >= spawnDuration) _isSpawning = false;
            }
        }

        private void SpawnProjectile()
        {
            var projectile = _projectilePool.Get();
            
            // get a position around the transform in a half arc to start the projectile, using unit circle
            var angle = Random.Range(0f, Mathf.PI);
            var x = Mathf.Cos(angle);
            var y = Mathf.Sin(angle);
            projectile.transform.position = transform.position + new Vector3(x, y, 0);
            
            var rb = projectile.GetComponent<Rigidbody2D>();
            var projectileBehaviour = projectile.GetComponent<ProjectileBehavior>();
            projectileBehaviour.Spawner = this;
            var direction = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));
            rb.linearVelocity = direction.normalized * projectileSpeed;
            rb.AddTorque( Random.Range(-1f, 1f) * torqueForce);
        }
        
        public void StartSpawning()
        {
            _spawnDurationTimer = 0f;
            _spawnTimer = 0f;
            _isSpawning = true;
        }

        public void ReturnProjectile(GameObject o)
        {
            _projectilePool.Release(o);
        }
    }
}