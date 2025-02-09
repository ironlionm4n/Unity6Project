using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AttackSequenceData[] attackSequences;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource sweepAttackSound;
    [SerializeField] private AudioSource sweepAttackChargeSound;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 1f;
    [SerializeField] private float staminaRefillDelay = 1f;
    [SerializeField] private float staminaRefillRate = 0.1f;

    [Header("Attack Combo Settings")]
    [SerializeField] private float attackSequenceDelay = 0.5f;
    [SerializeField] private Collider2D damageCollider;
    [SerializeField] private float missingStaminaModifier;

    // Internal stamina tracking
    private float _stamina;
    private float _staminaRefillTimer;

    // Internal combo tracking
    private int _attackSequenceIndex;     // Points to next attack in the combo
    private float _attackSequenceTimer;   // Tracks time since last attack

    // Example: reference to Animator if you have one
    private PlayerAnimation _animator;
    private static float _currentAttackDamage;
    private bool _isAttacking;
    public static float CurrentAttackDamage => _currentAttackDamage;

    private void Start()
    {
        // Initialize stamina
        _stamina = maxStamina;

        // Subscribe to your InputManager's event
        inputManager.OnAttackPressed += HandleAttack;
        inputManager.OnSweepAttackStarted += HandleSweepAttackStarted;
        inputManager.OnSweepAttackPerformed += HandleSweepAttack;
        inputManager.OnSweepAttackCanceled += HandleSweepAttackCanceled;
        _animator = GetComponentInChildren<PlayerAnimation>();
        damageCollider.enabled = false;
    }

    private void HandleSweepAttackStarted()
    {
        if(_isAttacking || _stamina < 0.25f) return;
        
        _animator.SetSweepCharging(true);
        sweepAttackChargeSound.PlayOneShot(sweepAttackChargeSound.clip);
    }

    private void OnDisable()
    {
        // Unsubscribe from your InputManager's event
        inputManager.OnAttackPressed -= HandleAttack;
        inputManager.OnSweepAttackStarted -= HandleSweepAttackStarted;
        inputManager.OnSweepAttackPerformed -= HandleSweepAttack;
        inputManager.OnSweepAttackCanceled -= HandleSweepAttackCanceled;
    }

    private void HandleSweepAttackCanceled()
    {
        _animator.SetSweepCharging(false);
    }

    private void HandleSweepAttack()
    {
        if (_stamina < 0.25f)
            return;
        sweepAttackSound.PlayOneShot(sweepAttackSound.clip);
        _animator.SetSweepCharging(false);
        _animator.SetAttackTrigger(PlayerAnimation.SweepAttack);
        _stamina -= 0.25f;
        staminaFill.fillAmount = GetRatioOfMaxStamina();
        _staminaRefillTimer = 0f;
        EnableDamageCollider();
        StartCoroutine(DisableDamageColliderRoutine(0.66f));
    }

    private void Update()
    {
        HandleStamina();
        HandleAttackSequenceReset();
    }

    private void HandleStamina()
    {
        // If we're not at full stamina, start the refill timer
        if (_stamina < maxStamina)
        {
            _staminaRefillTimer += Time.deltaTime;
            // After the delay, slowly move stamina back to full
            if (_staminaRefillTimer >= staminaRefillDelay)
            {
                float missingStamina = maxStamina - _stamina;
                float dynamicRefillRate = staminaRefillRate * Mathf.Log10(missingStamina + 1) * missingStaminaModifier;
                _stamina = Mathf.MoveTowards(_stamina, maxStamina, dynamicRefillRate * Time.deltaTime);
                
                staminaFill.fillAmount = _stamina / maxStamina;
            }
        }
    }

    private void HandleAttackSequenceReset()
    {
        // Count up how long since the last attack
        _attackSequenceTimer += Time.deltaTime;

        // If it exceeds the allowed delay between combo hits, reset
        if (_attackSequenceTimer >= attackSequenceDelay)
        {
            _attackSequenceIndex = 0;  // Next attack goes back to Attack1
        }
    }

    private void HandleAttack()
    {
        // If no AttackSequenceData objects exist, just return
        if (attackSequences == null || attackSequences.Length == 0 || _isAttacking)
            return;

        // Safety check to wrap around if _attackSequenceIndex is out of bounds
        if (_attackSequenceIndex >= attackSequences.Length)
        {
            _attackSequenceIndex = 0;
        }

        // Decide which attack to use
        AttackSequenceData currentAttackData = attackSequences[_attackSequenceIndex];

        // Check if we have enough stamina
        if (_stamina < currentAttackData.staminaCost)
        {
            // Not enough stamina for this attack
            return;
        }

        // Perform the attack:
        // 1. Deduct stamina
        _stamina -= currentAttackData.staminaCost;
        staminaFill.fillAmount = GetRatioOfMaxStamina();
        _staminaRefillTimer = 0f; // reset refill timer since we just attacked
        _currentAttackDamage = currentAttackData.damage;
        _animator.SetSweepCharging(false);

        // 2. Play the animation (if you have an animator)
        if (_animator)
        {
            attackSound.PlayOneShot(attackSound.clip);
            switch (currentAttackData.attackType)
            {
                case AttackType.Attack1:
                    _animator.SetAttackTrigger(PlayerAnimation.Attack1);
                    EnableDamageCollider();
                    break;
                case AttackType.Attack2:
                    _animator.SetAttackTrigger(PlayerAnimation.Attack2);
                    EnableDamageCollider();
                    break;
                // If you add cross slice or sweep in the array, handle them here
                case AttackType.CrossSlice:
                    _animator.SetAttackTrigger(PlayerAnimation.CrossSlice);
                    EnableDamageCollider();
                    break;
                // case AttackType.SweepAttack:
                //     _animator.SetAttackTrigger(PlayerAnimation.SweepAttack);
                //     break;
            }
        }

        StartCoroutine(DisableDamageColliderRoutine());

        // 3. Prepare for next combo step
        _attackSequenceIndex++;
        // reset timer to give the player a "window" to do the next attack
        _attackSequenceTimer = 0f;
    }

    public float GetRatioOfMaxStamina()
    {
        return _stamina / maxStamina;
    }

    /// <summary>
    /// Default delay is 0.215f, which is the duration of the basic attack combos
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator DisableDamageColliderRoutine(float delay = 0.215f)
    {
        _isAttacking = true;
        yield return new WaitForSeconds(delay);
        _isAttacking = false;
        DisableDamageCollider();
    }

    private void EnableDamageCollider()
    {
        if(PlayerMovement.FacingDirection == FacingDirection.Right)
            damageCollider.offset = new Vector2(1f, 0);
        else
            damageCollider.offset = new Vector2(-1f, 0);
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }
}

public enum AttackType
{
    Attack1,
    Attack2,
    CrossSlice,
    SweepAttack,
}