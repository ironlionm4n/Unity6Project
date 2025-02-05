using System.Collections;
using Enemies.Spider;
using UnityEngine;

namespace Enemies.Stategies
{
    public class BasicAttackStrategy : IEnemyStrategy
    {
        private ProjectileSpawner _projectileSpawner;
        private float _delay = 1f; // Bad because this is not flexible for different enemies
        private float _timer = 0f;
        private bool _isSpawning;
        private float _attackDuration = 2f;
        
        public void Execute(Enemy enemy)
        {
            if (!_isSpawning)
            {
                _timer += Time.deltaTime;
                if (_timer >= _delay)
                {
                    _isSpawning = true;
                    _timer = 0;
                    _projectileSpawner.StartSpawning();
                }
            }
            
            if (_isSpawning)
            {
                _attackDuration -= Time.deltaTime;
                if (_attackDuration <= 0)
                {
                    StopSpawning(enemy);
                }
            }
        }

        public void OnEnter(Enemy enemy)
        {
            enemy.TriggerAttack();
            _isSpawning = false;
            _timer = 0;
            _projectileSpawner = GameObject.FindFirstObjectByType<ProjectileSpawner>();
        }

        private void StopSpawning(Enemy enemy)
        {
            enemy.ChangeStrategy(new BasicPatrolStrategy());
        }

        public void OnExit(Enemy enemy)
        {
            _isSpawning = false;
            _attackDuration = 2f;
            _timer = 0;
        }
    }
}