﻿using UnityEngine;

namespace Enemies.Stategies
{
    public class BasicPatrolStrategy : IEnemyStrategy
    {
        private float _walkDuration;
        private float _walkTimer;
        private float _idleDuration;
        private float _idleTimer;
        private bool _isWalking;
        private float _moveDirection; // -1 for left, 1 for right
        private int _walkCount;
        private float _walkCountTimer;
        private float _walkCountDuration = 2f;
        private readonly float _wallRayLength;

        public BasicPatrolStrategy(float wallRayLength)
        {
            _wallRayLength = wallRayLength;
        }

        public void Execute(Enemy enemy)
        {
            if (_isWalking)
            {
                _walkCountTimer += Time.deltaTime;
                if (_walkCountTimer >= _walkCountDuration)
                {
                    _walkCountTimer = 0;
                    _walkCount++;
                    if (_walkCount >= 2)
                    {
                        enemy.ChangeToAttackStrategy();
                        return;
                    }
                }
                _walkTimer += Time.deltaTime;
                if (HitWall(enemy))
                {
                    SwitchDirections(enemy);
                }
                Move(enemy, _moveDirection);
                
                if (_walkTimer >= _walkDuration)
                {
                    _isWalking = false;
                    enemy.SetIsWalkingFalse();
                    _walkTimer = 0;
                }
            }
            else
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= _idleDuration)
                {
                    _walkTimer = 0;
                    _isWalking = true;
                    enemy.SetIsWalkingTrue();
                    _idleTimer = 0;
                    RandomizeWalkIdleDuration();
                    RandomizeMoveDirection(enemy);
                }
            }
        }

        private void SwitchDirections(Enemy enemy)
        {
            _moveDirection *= -1;
            enemy.transform.localScale = new Vector3(_moveDirection, 1, 1);
        }

        private bool HitWall(Enemy enemy)
        {
            var origin = enemy.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * _moveDirection, _wallRayLength, enemy.ObstacleLayer);
            Debug.DrawRay( origin, Vector2.right * (_moveDirection * _wallRayLength), Color.red);
            return hit.collider != null;
        }

        public void OnEnter(Enemy enemy)
        {
            RandomizeWalkIdleDuration();
            _idleTimer = 0;
            _walkTimer = 0;
            _isWalking = false;
            RandomizeMoveDirection(enemy);
        }

        private void RandomizeMoveDirection(Enemy enemy)
        {
            _moveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            enemy.transform.localScale = new Vector3(_moveDirection, 1, 1);
        }

        private void RandomizeWalkIdleDuration()
        {
            _walkDuration = Random.Range(2.5f, 5f);
            _idleDuration = Random.Range(1f, 2f);
        }

        public void OnExit(Enemy enemy)
        {
            StopWalking(enemy);
        }

        private void StopWalking(Enemy enemy)
        {
            _isWalking = false;
            enemy.SetIsWalkingFalse();
        }

        public void OnAttackReceived(Enemy enemy)
        {
            // Do nothing
            StopWalking(enemy);
            enemy.GetRigidbody2D.linearVelocity = Vector2.zero;
            _idleTimer = 0;
            _idleDuration = 0.75f;
        }

        private void Move(Enemy enemy, float dir)
        {
            float speed = 2f;
            enemy.GetRigidbody2D.linearVelocity = new Vector2(dir * speed, enemy.GetRigidbody2D.linearVelocity.y);
        }
    }
}