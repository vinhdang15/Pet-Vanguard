using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    GameSession gameSession;
    Rigidbody2D rigidody2D;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpSpeed = 15f;
    [SerializeField] Vector2 BounceBack = new(10f, 10f);
    [SerializeField] Slider healthSlider;

    [SerializeField] AudioSource runningAudioSoucre;
    [SerializeField] AudioSource jumpingAudioSoucre;
    [SerializeField] AudioSource pickedCoinAudioSoucre;


    BoxCollider2D m_BoxCollider;
    Vector2 moveInput;
    bool horizontalMove;
    bool isJump = false;

    bool isTouchingEnemy;
    public Animator animator;

    [Header("For Trade Pet")]
    NPCMovement[] npcArray;
    [SerializeField] GameObject canPayDialoguePanel;
    [SerializeField] GameObject unableToPayDialoguePanel;
    [SerializeField] TextMeshProUGUI NPCTypeInteractionText;
    GameObject CurrentNPCInteract;
    int CurentNPCType;

    [Header("Bush")]
    public bool isHiding = false;
    Vector2 bushCenter;
    GameObject currentBush;
    

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        rigidody2D = GetComponent<Rigidbody2D>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = gameSession.playerHealthMax;

        npcArray = FindObjectsOfType<NPCMovement>();

        MaintainNPC();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameSession.isGameOver)
        {
            Run();
            Jump();
            FlipSprite();
        }
        else
        {
            animator.SetBool("istakeDamage", false);
        }

        CheckCollisionWithNPCs();
        if(CurrentNPCInteract != null)
        {
            ShowDialoguePanel(CurrentNPCInteract);
        }

        updateHealthBar();
    }

    void MaintainNPC()
    {
        gameSession.LoadNPC();
    }

    void OnMove(InputValue Value)
    {
        moveInput = Value.Get<Vector2>();
        StartRunningSound();
    }

    void OnJump(InputValue Value)
    {
        if (Value.isPressed && m_BoxCollider.IsTouchingLayers(LayerMask.GetMask("Flatform", "TempSlab")))
        {
            rigidody2D.velocity += new Vector2(0f, jumpSpeed);
            StopRunningSound();
            jumpingAudioSoucre.Play();
        }
    }

    void Jump()
    {
        if (!m_BoxCollider.IsTouchingLayers(LayerMask.GetMask("Flatform", "TempSlab")))
        {
            isJump = true;
        }
        else
        {
            isJump = false;
        }
        animator.SetBool("isJumping", isJump);
    }

    void Run()
    {
        rigidody2D.velocity = new(moveInput.x * moveSpeed, rigidody2D.velocity.y);

        if (moveInput.x == 0f)
        {
            horizontalMove = false;
            StopRunningSound();
        }
        else if (moveInput.x != 0f)
        {
            horizontalMove = true;           
        }

        animator.SetBool("isRunning", horizontalMove);
    }

    void FlipSprite()
    {
        if (horizontalMove)
        {
            transform.localScale = new(Mathf.Sign(rigidody2D.velocity.x), 1f);
        }
    }

    // Animation event happen immediately when player trigger "takeDamage" for every second
    public void TakeDamage()
    {
        gameSession.ProcessPlayerTakeDamage();
    }

    // Trigger-animation is for event that happens once when it is been called
    // bool-animation is for event that continue happen when meet the condition
    // use Trigger-animation when TakeBulletDamage.
    // and use bool-animation when takedamage by stuck in trap, enemy, event touching enemy once then exit.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            animator.SetBool("istakeDamage", true);
            rigidody2D.velocity += new Vector2((BounceBack.x * Mathf.Sign(moveInput.x)), BounceBack.y);
            Debug.Log("onTrap");
        }
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            animator.SetTrigger("triggerTakeBulletDamage");
        }
        if (collision.gameObject.CompareTag("Gold") || collision.gameObject.CompareTag("Rubi"))
        {
            collision.gameObject.SetActive(false);
            Destroy(collision.gameObject);
            gameSession.TakeGem(collision.tag);
            pickedCoinAudioSoucre.Play();
        }
        if (collision.gameObject.CompareTag("Bush"))
        {
            currentBush = collision.gameObject;
            bushCenter = collision.bounds.center;
            EnterBush();
        }
        if (collision.gameObject.CompareTag("Enemy") && !isHiding)
        {
            animator.SetBool("istakeDamage", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            animator.SetBool("istakeDamage", false);
            
        }
        if (collision.gameObject.CompareTag("Bush"))
        {
            isHiding = false;
            GetComponent<SpriteRenderer>().enabled = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            animator.SetBool("istakeDamage", false);
        }
    }

    void CheckCollisionWithNPCs()
    {
        if (npcArray.Length == 0) return;
        foreach (NPCMovement npcScript in npcArray)
        {
            if (Vector2.Distance(npcScript.gameObject.transform.position, gameObject.transform.position) < 1f &&
                  npcScript.allowShowDialoguePanel)
            {
                CurrentNPCInteract = npcScript.gameObject;
                CurentNPCType = npcScript.npcType;
                return;
            }
            else
            {
                CurrentNPCInteract = null;
                canPayDialoguePanel.SetActive(false);
                unableToPayDialoguePanel.SetActive(false);
            }
        }      
    }

    void ShowDialoguePanel(GameObject CurrentNPCInteract)
    {
        if (CurrentNPCInteract.GetComponent<NPCMovement>().allowShowDialoguePanel)
        {
            if (CurrentNPCInteract.GetComponent<NPCMovement>().CanPlayerPay())
            {
                NPCTypeInteraction();
                canPayDialoguePanel.SetActive(true);
            }
            else unableToPayDialoguePanel.SetActive(true);
        }
    }

    void NPCTypeInteraction()
    {
        if(CurentNPCType == 0)
        {
            NPCTypeInteractionText.text = "I will protect you, there is no enemy will touch you, Let me make your armor!";
        }
        else if (CurentNPCType == 1)
        {
            NPCTypeInteractionText.text = "I can hunt down any enemy who wants to harm you. Let me be your weapon!";
        }      
    }

    public void Trade()
    {
        NPCMovement npcScript = CurrentNPCInteract.GetComponent<NPCMovement>();
        if (npcScript != null)
        {
            npcScript.Interact();
            canPayDialoguePanel.SetActive(false);
        }
        
    }

    void updateHealthBar()
    {
        healthSlider.value = gameSession.playerHealth;
    }

    void EnterBush()
    {
        isHiding = true;
        transform.position = bushCenter;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void StartRunningSound()
    {
        if (!runningAudioSoucre.isPlaying)
        {
            runningAudioSoucre.loop = true;
            runningAudioSoucre.Play();
        }
    }

    void StopRunningSound()
    { 
        runningAudioSoucre.Stop();
    }


}
