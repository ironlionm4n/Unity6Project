using System;
using UnityEngine;

namespace Player
{
    public class PlayerAttackCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Player collided with {other.name}");
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.HandleDamage(PlayerAttack.CurrentAttackDamage);
            }
        }
    }
}            