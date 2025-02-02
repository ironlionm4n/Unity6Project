using System;
using UnityEngine;

namespace Player
{
    public class PlayerAttackCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.HandleDamage(PlayerAttack.CurrentAttackDamage);
            }
        }
    }
}