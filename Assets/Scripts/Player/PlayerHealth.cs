using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float knockbackForce;
        
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        public void HandleDamage(float damage, float horizontalComponent)
        {
            health -= damage;
            Debug.Log($"Player health: {health}");
            Debug.Log($"Horiz Component: {horizontalComponent}");
            if (health <= 0)
            {
                Die();
            }
            else
            {
                // Knockback
                _rigidbody2D.AddForce(new Vector2(horizontalComponent, 0.05f) * knockbackForce, ForceMode2D.Impulse);
            }
        }
        
        private void Die()
        {
            Debug.Log("Player died");
        }
    }
}