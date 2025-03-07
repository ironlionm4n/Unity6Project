﻿using System;
using System.Collections;
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
        [SerializeField] private GameObject gameOver;
        [SerializeField] InputManager inputManager;
        
        private PlayerAnimation _playerAnimation;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        private float _currentHealth;
        private float _targetFill;
        private bool _isDead;
        private SpriteMask _deathMask;
        private Collider2D _collider;
        private ParticleSystem _deathParticles;
        public bool IsDead => _isDead;
        public bool RecoveringFromHit { get; set; }

        private void Awake()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _deathMask = GetComponentInChildren<SpriteMask>();
            _deathParticles = GetComponentInChildren<ParticleSystem>();
            _collider = GetComponent<Collider2D>();
            gameOver.SetActive(false);
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
            inputManager.OnDeath();
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
            _collider.enabled = false;
            _deathParticles.Play();
            StartCoroutine(ScaleDownMask());
        }

        private IEnumerator ScaleDownMask()
        {
            yield return new WaitForSeconds(.45f);
            var timeToScale = 1f;
            var currentScale = _deathMask.gameObject.transform.localScale;
            var targetScale = new Vector3(5, 5, 1);
            var elapsedTime = 0f;
            while (elapsedTime < timeToScale)
            {
                elapsedTime += Time.deltaTime;
                _deathMask.gameObject.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / timeToScale);
                yield return null;
            }
            gameOver.SetActive(true);
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