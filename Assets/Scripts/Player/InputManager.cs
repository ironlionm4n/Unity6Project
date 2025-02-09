using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    public Vector2 MovementInput { get; private set; }

    public Action OnAttackPressed;
    public Action OnJumpPressed;
    public Action OnSweepAttackPerformed;
    public Action OnSweepAttackStarted;
    public Action OnSweepAttackCanceled;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        
        // Subscribe to the Move action
        inputActions.Player.Move.performed += MoveOnPerformed();
        inputActions.Player.Move.canceled += MoveOnCanceled();
        inputActions.Player.Attack.performed += AttackOnPerformed();
        inputActions.Player.Jump.performed += JumpOnPerformed();
        inputActions.Player.SweepAttack.started += SweepAttackOnStarted();
        inputActions.Player.SweepAttack.performed += SweepAttackOnPerformed();
        inputActions.Player.SweepAttack.canceled += SweepAttackOnCanceled();
    }

    private Action<InputAction.CallbackContext> SweepAttackOnCanceled()
    {
        return _ => OnSweepAttackCanceled?.Invoke();
    }

    private Action<InputAction.CallbackContext> SweepAttackOnStarted()
    {
        return _ => OnSweepAttackStarted?.Invoke();
    }

    private Action<InputAction.CallbackContext> SweepAttackOnPerformed()
    {
        return _ => OnSweepAttackPerformed?.Invoke();
    }

    private Action<InputAction.CallbackContext> JumpOnPerformed()
    {
        return _ => OnJumpPressed?.Invoke();
    }

    private Action<InputAction.CallbackContext> AttackOnPerformed()
    {
        return _ => OnAttackPressed?.Invoke();
    }

    private Action<InputAction.CallbackContext> MoveOnCanceled()
    {
        return _ => MovementInput = Vector2.zero;
    }

    private Action<InputAction.CallbackContext> MoveOnPerformed()
    {
        return ctx => MovementInput = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        // Unsubscribe from the Move action
        inputActions.Player.Move.performed -= MoveOnPerformed();
        inputActions.Player.Move.canceled -= MoveOnCanceled();
        inputActions.Player.Attack.performed -= AttackOnPerformed();
        inputActions.Player.Jump.performed -= JumpOnPerformed();
        inputActions.Player.SweepAttack.performed -= SweepAttackOnPerformed();
        inputActions.Player.SweepAttack.started -= SweepAttackOnStarted();
        inputActions.Player.SweepAttack.canceled -= SweepAttackOnCanceled();
        inputActions.Player.Disable();
    }
}
