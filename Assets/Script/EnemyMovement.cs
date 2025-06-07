using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float patrolRange = 1f;
    [SerializeField] float detectionRange = 3f;
    [SerializeField] float chaseRange = 7f;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float fontCheckDistance = 0.1f;   
    float distanceFromInitial;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask flatformLayer;
    [SerializeField] Transform raycastCheckPoint;

    Rigidbody2D rb;
    Animator animator;
    Vector2 initialPosition;
    Transform playerTranform;
    PlayerMovement PlayerScript;
    public bool isChasing = false;
    bool isPatrolling = true;
    bool isTouchingPlayer = false;
    float direction;

    GameSession gameSession;

    private void Start()
    {
        animator = GetComponent<Animator>();
        gameSession = FindObjectOfType<GameSession>();
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        PlayerScript = FindObjectOfType<PlayerMovement>();

        // use initialization direction to get advanced on design enemy patrol direction at the start game
        direction = transform.localScale.x;
    }

    private void Update()
    {
        //isHitingPlayer();

        if (!gameSession.isGameOver && !isTouchingPlayer)
        {
            distanceFromInitial = Vector2.Distance(transform.position, initialPosition);

            if (isPatrolling)
            {
                Patrol(patrolSpeed);
            }
            else if (isChasing)
            {
                ChasePlayer();
            }
            DetectPlayer();
        }
        else if (isTouchingPlayer)
        {
            rb.velocity = Vector2.zero;           
        }
    }

    void Patrol(float speed)
    {
        rb.velocity = new(direction * speed, rb.velocity.y);
        transform.localScale = new(-direction, 1);

        ChangeDirection(distanceFromInitial >= patrolRange);

        if (HitWallOrBushOrTouchEdge())
        {
            ChangeDirection(true);
        }
    }

    void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerCollider != null && distanceFromInitial < chaseRange)
        {
            playerTranform = playerCollider.transform;
            bool playerHideState = playerCollider.transform.GetComponent<PlayerMovement>().isHiding;
            if (!playerHideState)
            {
                isChasing = true;
                isPatrolling = false;
            }
            else
            {
                isChasing = false;
                isPatrolling = true;
            }
        }
        else if (distanceFromInitial > chaseRange)
        {
            isChasing = false;
            isPatrolling = true;
        }
    }

    void ChasePlayer()
    {
        direction = Mathf.Sign(playerTranform.position.x - transform.position.x);
        if (!HitWallOrBushOrTouchEdge() || !playerTranform.GetComponent<PlayerMovement>().isHiding)
        {
            if (playerTranform is null) { return; }
            rb.velocity = new(direction * chaseSpeed, rb.velocity.y);
            transform.localScale = new(-direction, 1);
            if (distanceFromInitial < chaseRange)
            {
                isChasing = true;
                isPatrolling = false;
            }
        }
        else
        {
            isChasing = false;
            isPatrolling = true;
            Patrol(chaseSpeed);
        }
    }

    // to make sure when satisfy the change direction condition,
    // it will change "direction" only once before the next change direction condition meet,
    // not truning around in one spot(no bug)
    void ChangeDirection(bool condition)
    {
        if (condition && transform.position.x < initialPosition.x && direction < 0 ||
           condition && transform.position.x > initialPosition.x && direction > 0)
        {
            direction = -direction;
        }
    }

    bool HitWallOrBushOrTouchEdge()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(raycastCheckPoint.transform.position, Vector2.right * direction, fontCheckDistance, flatformLayer);
        RaycastHit2D TouchEdge = Physics2D.Raycast(raycastCheckPoint.transform.position, Vector2.down, groundCheckDistance, flatformLayer);
        Debug.DrawRay(transform.position, Vector2.down * new Vector2(0, 0.5f), Color.green);
        return wallHit.collider != null || TouchEdge.collider == null; 

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PlayerScript.isHiding)
        {
            if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("NPC")) isTouchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("NPC"))
        {
            isTouchingPlayer = false;
        }
    }

    //void isHitingPlayer()
    //{
    //    RaycastHit2D Hit = Physics2D.Raycast(raycastCheckPoint.transform.position, Vector2.right * direction, fontCheckDistance, playerLayer);
    //    isTouchingPlayer = (Hit.collider != null);
    //    PlayerScript.animator.SetBool("istakeDamage", isTouchingPlayer);
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(raycastCheckPoint.position, raycastCheckPoint.position + Vector3.down * groundCheckDistance);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(raycastCheckPoint.position, raycastCheckPoint.position + Vector3.right * fontCheckDistance);

    }
}
