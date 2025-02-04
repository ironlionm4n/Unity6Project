using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float knockbackForce;
        [SerializeField] private Color hurtColor;
        [SerializeField] private float hurtDuration;
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        public bool RecoveringFromHit { get; set; }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        public void HandleDamage(float damage, float horizontalComponent)
        {
            FlashHurtColor();
            health -= damage;
            Debug.Log($"Player health: {health}");
            Debug.Log($"Horiz Component: {horizontalComponent}");
            if (health <= 0)
            {
                Die();
            }
            else
            {
                RecoveringFromHit = true;
                _rigidbody2D.AddForce(new Vector2(horizontalComponent, 0.15f) * knockbackForce, ForceMode2D.Impulse);
            }
        }

        private void FlashHurtColor()
        {
            _spriteRenderer.color = hurtColor;
            Invoke(nameof(ResetColor), hurtDuration);
        }
        
        private void ResetColor()
        {
            RecoveringFromHit = false;
            _spriteRenderer.color = Color.white;
        }

        private void Die()
        {
            Debug.Log("Player died");
        }
    }
}