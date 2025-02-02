using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    public Vector2 MovementInput { get; private set; }

    public Action OnAttackPressed;
    public Action OnJumpPressed;

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
        
        inputActions.Player.Disable();
    }
}
