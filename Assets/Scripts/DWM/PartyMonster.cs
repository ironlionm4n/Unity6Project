using UnityEngine;

public class PartyMonster : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Adjust as needed
    private Vector3 targetPosition;
    private Animator animator;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }
    
    // Call this exactly once when you want the monster to start moving.
    public void StartMoving(Vector3 newTarget)
    {
        targetPosition = newTarget;
        
        // Determine the direction once at the start.
        Vector2 dir = (newTarget - transform.position).normalized;
        
        // If you only do 4-direction movement, you can round off:
        dir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
        
        // Update animator parameters so the monster faces the direction of movement
        animator.SetFloat(Horizontal, dir.x);
        animator.SetFloat(Vertical, dir.y);
    }

    private void Update()
    {
        // Smoothly move towards the target position
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    
}