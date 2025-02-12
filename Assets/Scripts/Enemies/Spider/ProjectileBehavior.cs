using System.Collections;
using Enemies.Spider;
using Player;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float damage = 10f;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.HandleDamage(damage, 0f);
            Spawner.ReturnProjectile(gameObject);
        }
    }

    private void OnEnable()
    {
        if(_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_collider2D == null)
            _collider2D = GetComponent<Collider2D>();
        
        _spriteRenderer.enabled = true;
        _collider2D.enabled = true;
        StartCoroutine(ReturnToPoolRoutine());
    }
    
    private IEnumerator ReturnToPoolRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        _spriteRenderer.enabled = false;
        _collider2D.enabled = false;
        // Look at making a streamline way to pool anything
        ParticleSystemManager.Instance.SpawnSpiderBombParticle(transform.position);
        Spawner.ReturnProjectile(gameObject);
    }

    public ProjectileSpawner Spawner { get; set; }
}
