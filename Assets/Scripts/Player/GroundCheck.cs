using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    private bool _justLanded;
    private PlayerAnimation _playerAnimation;

    public bool IsGrounded => Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }

    private void Awake()
    {
        _playerAnimation = GetComponentInParent<PlayerAnimation>();
    }

    private void FixedUpdate()
    {
        if (IsGrounded)
        {
            if(!_justLanded)
                OnLand();
            
        }
    }

    private void OnLand()
    { 
        _justLanded = true;
        _playerAnimation.JustLanded();
        Invoke(nameof(ResetJustLanded), 0.1f);
    }

    private void ResetJustLanded()
    {
        
    }
}
