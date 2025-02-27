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
    [SerializeField] private Direction startDirection;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject pauseMenu;
    
    private bool _isMoving;
    private Vector2 _movementInput;
    private InputAction _moveAction;
    private InputActionMap _terryActionMap;
    private Animator _animator;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private List<Animator> _partyAnimators;
    private Direction _lastMoveDirection;
    private Direction _nextMoveDirection;
    private Vector2 _lastMoveVector;
    private Vector3 _offset;
    private List<Vector3> _moveList;

    private void Awake()
    {
        _terryActionMap = terryInputActions.FindActionMap("Terry");
        _moveAction = _terryActionMap.FindAction("Move");
        _terryActionMap.Enable();
        _moveAction.Enable();
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPause;
        _animator = GetComponent<Animator>();
        _partyAnimators = new List<Animator>();
        _offset = startDirection switch
        {
            Direction.Up => Vector2.down,
            Direction.Down => Vector2.up,
            Direction.Left => Vector2.right,
            Direction.Right => Vector2.left,
            _ => Vector2.zero
        };
        foreach (var member in party)
        {
            _partyAnimators.Add(member.GetComponent<Animator>());
        }
        _lastMoveDirection = Direction.Right;
        _moveList = new List<Vector3>();
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    private void Start()
    {
        // for(int i = 0; i < party.Count; i++)
        // {
        //     party[i].SetActive(true);
        //     party[i].transform.position = transform.position + _offset * (i + 1);
        // }
    }

    private void OnDisable()
    {
        _terryActionMap.Disable();
        _moveAction.Disable();
        pauseAction.action.Disable();
        pauseAction.action.performed -= OnPause;
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
        } else if (moveDirection.y != 0) {
            moveDirection.x = 0;
        }

        if (moveDirection.sqrMagnitude > 0.5f)
        {
            moveDirection.Normalize();
            _animator.SetFloat(Horizontal, moveDirection.x);
            _animator.SetFloat(Vertical, moveDirection.y);
            
            if (CanMoveInDirection(moveDirection))
            {
                StartCoroutine(MoveTerryAndParty(moveDirection));
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

    private IEnumerator MoveTerryAndParty(Vector2 direction)
    {
        var origin = transform.position;
        _moveList.Add(origin);
        var target = origin + (Vector3)direction;
        var counter = 0;
        // Move Terry
        while (Vector2.Distance(transform.position, target) > Mathf.Epsilon)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, maxMoveDelta);

            yield return null;
        }
        transform.position = target;
        
        for (int i = 0; i < Mathf.Min(party.Count, _moveList.Count); i++)
        {
            var followerTarget = _moveList[ Mathf.Max(0, _moveList.Count - i - 1)];
            party[i].GetComponent<PartyMonster>().StartMoving(followerTarget);
        }

        
        if(_moveList.Count > party.Count)
        {
            _moveList.RemoveAt(0);
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