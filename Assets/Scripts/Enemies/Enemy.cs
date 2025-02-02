using System;
using Enemies.Stategies;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float invulnerabilityTime = 0.75f;
    [SerializeField] private float health = 100f;
    [SerializeField] private Transform visualCenter;
    private Animator _animator;
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private float _invulnerabilityTimer;
    private bool IsInvulnerable => _invulnerabilityTimer > 0;
    private IEnemyStrategy _currentStrategy;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private static readonly int Death = Animator.StringToHash("Death");
    public Rigidbody2D GetRigidbody2D => _rigidbody2D;
    public LayerMask ObstacleLayer => obstacleLayer;
    public Collider2D GetCollider2D => _collider2D;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        ChangeStrategy(new BasicPatrolStrategy());
    }

    private void Update()
    {
        if (_invulnerabilityTimer > 0)
        {
            _invulnerabilityTimer -= Time.deltaTime;
        }
        
        _currentStrategy.Execute(this);
    }
    
    public void ChangeStrategy(IEnemyStrategy newStrategy)
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.OnExit(this);
        }
        _currentStrategy = newStrategy;
        _currentStrategy.OnEnter(this);
    }
    
    [ContextMenu("Trigger Damage")]
    public void TriggerDamage()
    {
        _invulnerabilityTimer = invulnerabilityTime;
        if(_currentStrategy is BasicPatrolStrategy basicPatrolStrategy)
            basicPatrolStrategy.OnAttackReceived(this);
        _animator.SetTrigger(Damage);
    }
    
    [ContextMenu("Set IsWalking True")]
    public void SetIsWalkingTrue()
    {
        _animator.SetBool(IsWalking, true);
    }
    
    [ContextMenu("Set IsWalking False")]
    public void SetIsWalkingFalse()
    {
        _animator.SetBool(IsWalking, false);
    }
    
    public void TriggerDeathAnimation()
    {
        _animator.SetTrigger(Death);
    }

    public void HandleDamage(float damage)
    {
        if(_currentStrategy is BasicDeathStrategy) return;
        
        if(IsInvulnerable) return;
        
        // Handle damage here
        Debug.Log($"Dealt {damage} damage to enemy");
        health -= damage;
        if (health <= 0)
        {
            TriggerDeath();
        }
        else
        {
            TriggerDamage();
        }
         
    }

    public void TriggerDeath()
    {
        ChangeStrategy(new BasicDeathStrategy());
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision detected");
        var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            var direction = other.transform.position - visualCenter.position;
            playerHealth.HandleDamage(10f, direction.normalized.x);
        }
    }
}
