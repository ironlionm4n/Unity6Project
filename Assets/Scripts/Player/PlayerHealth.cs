using System;
using Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float knockbackForce;
        [SerializeField] private Color hurtColor;
        [SerializeField] private float hurtDuration;
        [SerializeField] private AudioSource hurtSound;
        [SerializeField] private Image healthFill;
        [SerializeField] private float fillSpeed;
        [SerializeField]
        private CinemachineManager cinemachineManager;
        
        private PlayerAnimation _playerAnimation;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        private float _currentHealth;
        private float _targetFill;
        private bool _isDead;
        public bool IsDead => _isDead;
        public bool RecoveringFromHit { get; set; }

        private void Awake()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        private void Start()
        {
            _currentHealth = health;
            _targetFill = _currentHealth;
        }

        private void Update()
        {
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, _targetFill, Time.deltaTime * fillSpeed);
        }

        public void HandleDamage(float damage, float horizontalComponent)
        {
            FlashHurtColor();
            _currentHealth -= damage;
            CalculateRatioOfHealth();
            if (_currentHealth <= 0 && !_isDead)
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
            hurtSound.PlayOneShot(hurtSound.clip);
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
            _isDead = true;
            _playerAnimation.SetDeathTrigger();
            cinemachineManager.MakePlayerDeathCinemachinePriority();
        }
        
        private void CalculateRatioOfHealth()
        {
            // 1 * (current health / max health) = ratio of health remaining (0-1) 
            // max fill of the image is 1, this calculates the ratio of health remaining from 0-1
            _targetFill = 1 * (_currentHealth / health);
            // Clamp the fill amount to 0-1
            _targetFill = Mathf.Clamp(_targetFill, 0, 1);
        }
        
        public void GainHealth(float amount)
        {
            _currentHealth += amount;
            CalculateRatioOfHealth();
        }
    }
}