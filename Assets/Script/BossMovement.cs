using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMovement : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask flatformLayer;
    [SerializeField] LayerMask NPCBullet;
    [SerializeField] Transform groundCheck;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpInterval = 1f;

    public bool playerDectected;
    bool isGrounded;

    Animator animator;
    Rigidbody2D bossRigidbody2D;
    BoxCollider2D boxCollider2D;
    GameSession gameSession;
    LevelChanger levelChanger;

    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        levelChanger = FindObjectOfType<LevelChanger>();
        animator = GetComponent<Animator>();
        bossRigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        if (!gameSession.isGameOver)
        {
            DetechPlayer();
            CheckIfGrounded();
            CheckToShooting();
        }       
    }

    void DetechPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null)
        {
            playerDectected = true;
            levelChanger.GetComponentInChildren<Transform>().Find("Canvas - Level Changer/Text (TMP) - BossEncounterTimer").gameObject.SetActive(true);
            GetComponent<BossEncounterTimer>().StartCountdown();
        }
        //else
        //{
            // after see player the boss keep jumping and shootting
            //playerDectected = false;
        //}
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, flatformLayer);      
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            if(playerDectected && isGrounded)
            {
                //JumpRandomDirection();
                JumpTowradPlayer();
                animator.SetTrigger("isJump");
            }
            yield return new WaitForSeconds(jumpInterval);
        }
    }

    void JumpRandomDirection()
    {
        int randomNumber = Random.Range(0, 2);
        int direction = (randomNumber == 0) ? 1 : -1;

        Vector2 jumpVector = new Vector2(direction, 1);
        // Tính vật lý phù hợp hơn rigidbody2D.velocity
        bossRigidbody2D.AddForce(jumpVector.normalized * jumpForce, ForceMode2D.Impulse);
    }

    void JumpTowradPlayer()
    {
        float playerX = FindFirstObjectByType<PlayerMovement>().gameObject.transform.position.x;
        float jumpDirect = (transform.position.x - playerX) > 0 ? -1 : 1;
        Vector2 jumpVector = new Vector2(jumpDirect, 1).normalized;
        bossRigidbody2D.AddForce(jumpVector * jumpForce, ForceMode2D.Impulse);

        transform.localScale = new(-jumpDirect, 1);

    }

    void CheckToShooting()
    {
        Vector2 playerPosition = FindObjectOfType<PlayerMovement>().transform.position;
        if (Mathf.Abs(transform.position.y - 0.239f - playerPosition.y) < 0.5f)
        {
            animator.SetTrigger("isAttack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
