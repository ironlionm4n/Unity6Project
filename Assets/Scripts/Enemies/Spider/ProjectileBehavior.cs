using System.Collections;
using Enemies.Spider;
using Player;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float damage = 10f;
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
        StartCoroutine(ReturnToPoolRoutine());
    }
    
    private IEnumerator ReturnToPoolRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Spawner.ReturnProjectile(gameObject);
    }

    public ProjectileSpawner Spawner { get; set; }
}
