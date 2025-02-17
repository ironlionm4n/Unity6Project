using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public event Action OnSweepAttackStarted;
    public event Action OnSweepAttackPerformed;
    public Action OnSweepAttackCanceled;
    
    [SerializeField] private Image staminaFill;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AttackSequenceData[] attackSequences;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource sweepAttackSound;
    [SerializeField] private AudioSource sweepAttackChargeSound;
    [Header("Light Intensity"), SerializeField] private float lowLightValue = 1f;
    [SerializeField] private float highLightValue = 5f;

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
    
    private PlayerAnimation _playerAnimation;
    private PlayerHealth _playerHealth;
    private Light2D _playerLight;
    private SpriteRenderer _playerSpriteRenderer;
    private static float _currentAttackDamage;
    private bool _isAttacking;
    private bool _sweepAttackStarted;
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
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerHealth = GetComponent<PlayerHealth>();
        damageCollider.enabled = false;
        _playerLight = GetComponentInChildren<Light2D>();
        _playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void HandleSweepAttackStarted()
    {
        if(_isAttacking || _stamina < 0.25f || CheckAnimatorStatesForBasicAttacks()) return;
        
        _sweepAttackStarted = true;
        _playerAnimation.SetSweepCharging(true);
        OnSweepAttackStarted?.Invoke();
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
        _sweepAttackStarted = false;
        OnSweepAttackCanceled?.Invoke();
        _playerAnimation.SetSweepCharging(false);
    }

    private void HandleSweepAttack()
    {
        if (_stamina < 0.25f || CheckAnimatorStatesForBasicAttacks() || !_sweepAttackStarted)
            return;
        
        sweepAttackSound.PlayOneShot(sweepAttackSound.clip);
        _sweepAttackStarted = false;
        OnSweepAttackPerformed?.Invoke();
        _playerAnimation.SetSweepCharging(false);
        _playerAnimation.SetAttackTrigger(PlayerAnimation.SweepAttack);
        _stamina -= 0.25f;
        staminaFill.fillAmount = GetRatioOfMaxStamina();
        _staminaRefillTimer = 0f;
        EnableDamageCollider();
        StartCoroutine(DisableDamageColliderRoutine(0.66f));
    }

    public bool CheckAnimatorStatesForBasicAttacks()
    {
        return _playerAnimation.IsPlaying(PlayerAnimation.Attack1) || _playerAnimation.IsPlaying(PlayerAnimation.Attack2) || _playerAnimation.IsPlaying(PlayerAnimation.CrossSlice);
    }

    private void Update()
    {
        if(_playerHealth.IsDead) return;
        
        HandleStamina();
        HandleAttackSequenceReset();
        SetLightIntensity();
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
        _isAttacking = true;

        // 2. Play the animation
        if (_playerAnimation)
        {
            attackSound.PlayOneShot(attackSound.clip);
            switch (currentAttackData.attackType)
            {
                case AttackType.Attack1:
                    _playerAnimation.SetAttackTrigger(PlayerAnimation.Attack1);
                    EnableDamageCollider();
                    break;
                case AttackType.Attack2:
                    _playerAnimation.SetAttackTrigger(PlayerAnimation.Attack2);
                    EnableDamageCollider();
                    break;
                // If you add cross slice or sweep in the array, handle them here
                case AttackType.CrossSlice:
                    _playerAnimation.SetAttackTrigger(PlayerAnimation.CrossSlice);
                    EnableDamageCollider();
                    break;
            }
        }

        StartCoroutine(DisableDamageColliderRoutine());

        // 3. Prepare for next combo step
        _attackSequenceIndex++;
        // reset timer to give the player a "window" to do the next attack
        _attackSequenceTimer = 0f;
    }

    private void SetLightIntensity()
    {
        // want the light inentisty to range from 1 to 5 depending on ratio
        _playerLight.intensity = lowLightValue + highLightValue * GetRatioOfMaxStamina();
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
}