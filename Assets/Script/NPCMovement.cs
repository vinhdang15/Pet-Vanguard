using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCMovement : MonoBehaviour
{

    [SerializeField] public int npcType;
    [SerializeField] TextMeshProUGUI petGoldRequireText;
    [SerializeField] TextMeshProUGUI petRubiRequireText;
    [SerializeField] float detectionRange = 5f;
    public int petGoldRequire = 2;
    public int petRubiRequire = 1;

    PlayerMovement playerMovement;
    GameSession gameSession;
    Transform FollowPoint;
    Transform NpcFontPoint;
    Transform NpcRearPoint;
    bool isFollowingPlayer = false; 

    public bool allowShowDialoguePanel = true;

    GameObject currentEnemy;
    bool isEnemyActive;

    [Header("Option NPC can Shoot")]
    [SerializeField] bool canShoot;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameSession = FindObjectOfType<GameSession>();
        FollowPoint = playerMovement.transform.Find("NPCFollowPoint").gameObject.transform;
        NpcFontPoint = playerMovement.transform.Find("NpcFontPoint").gameObject.transform;
        NpcRearPoint = playerMovement.transform.Find("NpcRearPoint").gameObject.transform;
        ShowPriceCanvas();

        if(npcType == 0) canShoot = false;
        else if(npcType == 1) canShoot = true;
    }

    private void Update()
    {
        if (!gameSession.isGameOver)
        {
            if (gameSession.isGameOver) return;

            if (!canShoot)
            {
                HandleNonShootingBehavior();
            }
            else
            {
                HandleShootingBehavior();
            }

            if (isFollowingPlayer)
            {
                CheckActiveEnemy();
            }
        }             
    }

    void ShowPriceCanvas()
    {
        petGoldRequireText.text = petGoldRequire.ToString();
        petRubiRequireText.text = petRubiRequire.ToString();
    }

    public bool CanPlayerPay()
    {
        return gameSession.playerGold >= petGoldRequire && gameSession.playerRubi >= petRubiRequire;
    }    
    public void Interact()
    {
        if (canShoot) PlayerPrefs.SetInt("NPC_ATK", npcType);
        else PlayerPrefs.SetInt("NPC_DEF", npcType);

        if (CanPlayerPay())
        {
            gameSession.playerGold -= petGoldRequire;
            gameSession.playerRubi -= petRubiRequire;
            isFollowingPlayer = true;
            FollowState();
        }
    }

    public void FollowState()
    {   
        isFollowingPlayer = true;
        gameObject.GetComponent<Animator>().SetTrigger("isFlying");
        gameObject.GetComponentInChildren<Transform>().Find("Canvas - PetPrice").gameObject.SetActive(false);
        // avoid showing Canvas-DialoguePanel when player hit pet again
        allowShowDialoguePanel = false;
    }
    void FollowPlayer(Transform point)
    {
        if (isFollowingPlayer)
        {           
            transform.position = Vector2.Lerp(transform.position, new Vector2(point.position.x, point.position.y), 0.2f);
        }
    }

    void CheckActiveEnemy()
    {
        EnemyMovement[] enemiesList = FindObjectsOfType<EnemyMovement>();
        BossMovement[] bossList = FindObjectsOfType<BossMovement>();
        if (enemiesList.Length > 0)
        {
            foreach (EnemyMovement enemy in enemiesList)
            {
                //if(enemy.isChasing is true)
                if (Mathf.Abs(playerMovement.transform.position.x - enemy.transform.position.x) < detectionRange &&
                    Mathf.Abs(playerMovement.transform.position.y - enemy.transform.position.y) < 0.5f)
                {
                    currentEnemy = enemy.gameObject;
                    isEnemyActive = true;
                    return;
                }
            }
            isEnemyActive = false;
            currentEnemy = null;
            transform.localScale = playerMovement.transform.localScale;
        }
        else if (bossList.Length > 0)
        {
            foreach (BossMovement boss in bossList)
            {
                //if (boss.playerDectected is true)
                if(Mathf.Abs(playerMovement.transform.position.x - boss.transform.position.x) < detectionRange)
                {
                    currentEnemy = boss.gameObject;
                    isEnemyActive = true;
                    return;
                }
            }
            isEnemyActive = false;
            currentEnemy = null;
            transform.localScale = playerMovement.transform.localScale;

        }
        else
        {
            // if there is no enemy in map
            isEnemyActive = false;
            currentEnemy = null;
            transform.localScale = playerMovement.transform.localScale;
        }
    }

    void GetToShootingPoint()
    {
        if (currentEnemy != null)
        {
            // alway head to enemy and at the enemy side relative to player
            float enemyDirection = Mathf.Sign(currentEnemy.transform.position.x - playerMovement.transform.position.x);
            transform.localScale = new Vector2(enemyDirection, 1f);

            if (enemyDirection > 0)
            {
                if (playerMovement.transform.localScale.x < 0) { FollowPlayer(NpcFontPoint); }
                else { FollowPlayer(NpcRearPoint); }
            }
            else
            {
                if (playerMovement.transform.localScale.x > 0) { FollowPlayer(NpcFontPoint); }
                else { FollowPlayer(NpcRearPoint); }
            }
        }
    }

    void GetToDefensePoint()
    {
        if(currentEnemy != null)
        {
            // alway head to enemy and at the enemy side relative to player
            float enemyDirection = Mathf.Sign(currentEnemy.transform.position.x - playerMovement.transform.position.x);
            transform.localScale = new Vector2(enemyDirection, 1f);

            if (enemyDirection > 0)
            {
                if (playerMovement.transform.localScale.x > 0) { FollowPlayer(NpcFontPoint); }
                else { FollowPlayer(NpcRearPoint); }
            }
            else
            {
                if (playerMovement.transform.localScale.x < 0) { FollowPlayer(NpcFontPoint); }
                else { FollowPlayer(NpcRearPoint); }
            }
        }   
    }

    void HandleNonShootingBehavior()
    {
        if (isFollowingPlayer && !isEnemyActive || playerMovement.isHiding)
        {
            FollowPlayer(NpcRearPoint);
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else if (isFollowingPlayer && isEnemyActive)
        {
            GetToDefensePoint();
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    void HandleShootingBehavior()
    {
        if ((isFollowingPlayer && !isEnemyActive) || playerMovement.isHiding)
        {
            FollowPlayer(FollowPoint);           
            GetComponent<ShootingSystem>().SetShootingState(false);
        }
        else if (isFollowingPlayer && isEnemyActive)
        {
            GetToShootingPoint();
            GetComponent<ShootingSystem>().SetShootingState(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerMovement != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerMovement.transform.position, detectionRange);
            Gizmos.color = Color.blue;
        }       
    }
}
