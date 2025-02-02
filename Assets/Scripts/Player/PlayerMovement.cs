using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")] [SerializeField]
    private float defaultMoveSpeed = 5f;

    [SerializeField] private float slowdownSpeed = 2f; // Speed when slowed by attack
    [SerializeField] private float slowdownDuration = 0.4f;
    [SerializeField] private float speedRecoverRate = 5f; // How quickly we go back to full speed
    [SerializeField] private InputManager inputManager;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private static FacingDirection _facingDirection;
    public static FacingDirection FacingDirection => _facingDirection;
    
    // Internal speed states
    private float _currentMoveSpeed;
    private float _slowdownTimer; // Tracks how long we've been slowed
    private bool _isSlowed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Subscribe to the OnAttackPressed event
        inputManager.OnAttackPressed += HandleAttack;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnAttackPressed event
        inputManager.OnAttackPressed -= HandleAttack;
    }

    private void HandleAttack()
    {
        _currentMoveSpeed = slowdownSpeed;
        _isSlowed = true;
        _slowdownTimer = 0f;
    }

    private void Update()
    {
        HandleSpeedRecovery();
        HandleSpriteXFlip();
    }

    private void FixedUpdate()
    {
        var movementInput = inputManager.MovementInput;
        _rb.linearVelocity = new Vector2(movementInput.x * _currentMoveSpeed, _rb.linearVelocity.y);
    }

    private void HandleSpriteXFlip()
    {
        if (inputManager.MovementInput.x > 0 && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = false;
            _facingDirection = FacingDirection.Right;
        }
        else if (inputManager.MovementInput.x < 0 && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = true;
            _facingDirection = FacingDirection.Left;
        }
    }

    private void HandleSpeedRecovery()
    {
        // If we are currently slowed, count up how long we've been slowed.
        if (_isSlowed)
        {
            _slowdownTimer += Time.deltaTime;
            // Once slowdownDuration expires, we start speeding back up
            if (_slowdownTimer >= slowdownDuration)
            {
                // Move _currentMoveSpeed toward defaultMoveSpeed at a set rate
                _currentMoveSpeed = Mathf.MoveTowards(
                    _currentMoveSpeed,
                    defaultMoveSpeed,
                    speedRecoverRate * Time.deltaTime
                );

                // If we’ve reached default speed, we’re no longer slowed
                if (Mathf.Abs(_currentMoveSpeed - defaultMoveSpeed) < 0.01f)
                {
                    _currentMoveSpeed = defaultMoveSpeed;
                    _isSlowed = false;
                }
            }
        }
        else
        {
            // Not slowed: ensure we’re at default speed (or heading there)
            _currentMoveSpeed = defaultMoveSpeed;
        }
    }
}

public enum FacingDirection
{
    Right,
    Left
}