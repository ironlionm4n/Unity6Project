using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    
    public bool IsGrounded => Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
