using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terry : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float rayLength = 0.55f;
    [SerializeField] InputActionAsset terryInputActions;
    [SerializeField] private float maxMoveDelta = 0.05f;
    [SerializeField] private List<GameObject> party;
    private bool _isMoving;
    private Vector2 _movementInput;
    private InputAction _moveAction;
    private InputActionMap _terryActionMap;
    private Animator _animator;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private Vector3 _offset;
    private List<Animator> _partyAnimators;
    private Direction _lastMoveDirection;
    private Direction _nextMoveDirection;
    private Vector2 _lastMoveVector;

    private void Awake()
    {
        _terryActionMap = terryInputActions.FindActionMap("Terry");
        _moveAction = _terryActionMap.FindAction("Move");
        _terryActionMap.Enable();
        _moveAction.Enable();
        _animator = GetComponent<Animator>();
        _partyAnimators = new List<Animator>();
        foreach (var member in party)
        {
            _partyAnimators.Add(member.GetComponent<Animator>());
        }
        _lastMoveDirection = Direction.Right;
    }

    private void Start()
    {
        for(int i = 1; i < party.Count; i++)
        {
            party[i].SetActive(true);
            party[i].transform.position = transform.position + _offset;
        }
    }

    private void OnDisable()
    {
        _terryActionMap.Disable();
        _moveAction.Disable();
    }

    private void Update()
    {
        if(_isMoving) return;

        HandleMove(_moveAction.ReadValue<Vector2>());
    }

    public void HandleMove(Vector2 moveDirection)
    {
        _isMoving = true;
        if (moveDirection.x != 0) {
            moveDirection.y = 0;
            _nextMoveDirection = moveDirection.x > 0 ? Direction.Right : Direction.Left;
            _offset = moveDirection.x > 0 ? Vector3.right : Vector3.left;
        } else if (moveDirection.y != 0) {
            moveDirection.x = 0;
            _nextMoveDirection = moveDirection.y > 0 ? Direction.Up : Direction.Down;
            _offset = moveDirection.y > 0 ? Vector3.up : Vector3.down;
        }

        if (moveDirection.sqrMagnitude > 0.5f)
        {
            _animator.SetFloat(Horizontal, moveDirection.x);
            _animator.SetFloat(Vertical, moveDirection.y);
            _lastMoveDirection = moveDirection.x > 0 ? Direction.Right : moveDirection.x < 0 
                ? Direction.Left : moveDirection.y > 0 ? Direction.Up : Direction.Down;
            _lastMoveVector = _lastMoveDirection switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => Vector2.zero
            };
            
            if (CanMoveInDirection(moveDirection.normalized))
            {
                StartCoroutine(MoveTerryAndParty(moveDirection.normalized));
            }
            else
            {
                _isMoving = false;
            }
        }
        else
        {
            _isMoving = false;
        }
    }

    private IEnumerator MoveTerryAndParty(Vector2 newDirectionVector)
    {
        // var origin = transform.position;
        // var target = origin + (Vector3)direction.normalized;
        // while (Vector2.Distance(transform.position, target) > Mathf.Epsilon)
        // {
        //     transform.position = Vector2.MoveTowards(transform.position, target, maxMoveDelta);
        //     yield return null;
        // }
        // transform.position = target;
        // _isMoving = false;

        foreach (var partyAnimator in _partyAnimators)
        {
            if(CanMoveInDirection(_nextMoveDirection))
            {
                partyAnimator.SetFloat(Horizontal, newDirectionVector.x);
                partyAnimator.SetFloat(Vertical, newDirectionVector.y);
            }
            else
            {
                partyAnimator.SetFloat(Horizontal, _lastMoveVector.x);
                partyAnimator.SetFloat(Vertical, _lastMoveVector.y);
            }
        }
        
        var origin = transform.position;
        var target = origin + (Vector3)newDirectionVector.normalized;
        while (Vector2.Distance(transform.position, target) > Mathf.Epsilon)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, maxMoveDelta);
            for (int i = 1; i < party.Count; i++)
            {
                party[i].transform.position = transform.position + _offset;
            }
            yield return null;
        }
        transform.position = target;
        for (int i = 1; i < party.Count; i++)
        {
            party[i].transform.position = target + _offset;
        }
        _isMoving = false;
    }

    private bool CanMoveInDirection(Direction nextMoveDirection)
    {
        switch (nextMoveDirection)
        {
            case Direction.Up:
                return CanMoveInDirection(Vector2.up);
            case Direction.Down:
                return CanMoveInDirection(Vector2.down);
            case Direction.Left:
                return CanMoveInDirection(Vector2.left);
            case Direction.Right:
                return CanMoveInDirection(Vector2.right);
            default:
                return false;
        }
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        var origin = transform.position;
        var hit = Physics2D.Raycast(origin, direction, rayLength, collisionLayer);
        Debug.DrawRay(origin, direction * rayLength, Color.red);
        Debug.Log($"Hit: {hit.collider == null}");
        return hit.collider == null;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}