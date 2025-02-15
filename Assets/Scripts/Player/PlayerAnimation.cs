using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    private PlayerJump _playerJump;
    private Animator _animator;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    public static readonly int Attack1 = Animator.StringToHash("Attack_1");
    public static readonly int Attack2 = Animator.StringToHash("Attack_2");
    public static readonly int CrossSlice = Animator.StringToHash("CrossSlice");
    public static readonly int SweepAttack = Animator.StringToHash("SweepAttack");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int SweepCharging = Animator.StringToHash("SweepCharging");


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerJump = GetComponent<PlayerJump>();
    }

    private void OnEnable()
    {
        // Subscribe to the OnAttackPressed event
        inputManager.OnJumpPressed += OnJumpPressed;
        _playerJump.OnLand += OnLand;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from the OnAttackPressed event
        inputManager.OnJumpPressed -= OnJumpPressed;
        _playerJump.OnLand -= OnLand;
    }

    private void OnLand()
    {
        _animator.SetBool(IsJumping, false);
        _animator.SetTrigger("Land");
    }

    private void OnJumpPressed()
    {
        _animator.SetTrigger(Jump);
        _animator.SetBool(IsJumping, true);
    }

    private void Update()
    {
        var movementInput = Input.GetAxis("Horizontal");
        _animator.SetFloat(Horizontal, Mathf.Abs(movementInput));
    }

    public void SetAttackTrigger(int attackTrigger)
    {
        _animator.SetTrigger(attackTrigger);
    }

    public void SetSweepCharging(bool b)
    {
        _animator.SetBool(SweepCharging, b);
    }

    public void JustLanded()
    {
        _animator.SetBool(IsJumping, false);
    }
}
