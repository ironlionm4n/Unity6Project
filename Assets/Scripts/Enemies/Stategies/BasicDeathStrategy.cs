using UnityEngine;

namespace Enemies.Stategies
{
    public class BasicDeathStrategy : IEnemyStrategy
    {
        private float _deathDuration;
        private float _deathTimer;
        
        public void Execute(Enemy enemy)
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= _deathDuration)
            {
                enemy.DestroyEnemy();
            }
        }

        public void OnEnter(Enemy enemy)
        {
            _deathDuration = 2.5f;
            enemy.GetRigidbody2D.linearVelocity = Vector2.zero;
            enemy.GetRigidbody2D.angularVelocity = 0;
            enemy.GetRigidbody2D.gravityScale = 0;
            enemy.GetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            enemy.TriggerDeathAnimation();
            enemy.GetCollider2D.enabled = false;
        }

        public void OnExit(Enemy enemy)
        {
        }
    }
}