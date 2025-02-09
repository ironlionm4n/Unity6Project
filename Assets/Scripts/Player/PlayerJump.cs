using System;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public Action OnLand;
    
    [SerializeField] InputManager inputManager;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float downForceIncreaseRate = 2f;
    [SerializeField] private float maxDownForce = 10f;
    [SerializeField] private AudioSource jumpSound;
    private bool _isJumping;
    private Rigidbody2D _rb;
    private float _downForce = 0f;
    private GroundCheck _groundCheck;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void OnEnable()
    {
        inputManager.OnJumpPressed += HandleJump;
    }

    private void FixedUpdate()
    {
        if (_isJumping)
        {
            _downForce = Mathf.MoveTowards(_downForce, maxDownForce, downForceIncreaseRate * Time.fixedDeltaTime);
            _rb.AddForce(Vector2.down * _downForce, ForceMode2D.Force);
        }
        else
        {
            _downForce = 0f;
        }
    }

    private void HandleJump()
    {
        if (!_isJumping && _groundCheck.IsGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _isJumping = true;
        jumpSound.PlayOneShot(jumpSound.clip);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isJumping = false;
            OnLand?.Invoke();
        }
    }
}
