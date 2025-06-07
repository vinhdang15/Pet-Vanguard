using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI PlayerGoldText;
    [SerializeField] TextMeshProUGUI PlayerRubiText;
    [SerializeField] TextMeshProUGUI countdownText;

    int playerHealth;
    int playerGold;
    int playerRubi;

    GameSession gameSession;
    BossEncounterTimer bossEncounterTimer;
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentPlayerHealth();
        GetPlayerGold();
        GetPlayerRubi();
        UpdateCurrentTime();
    }

    void GetCurrentPlayerHealth()
    {
        playerHealth = gameSession.playerHealth;
        playerHealthText.text = playerHealth.ToString();
    }

    void GetPlayerGold()
    {
        playerGold = gameSession.playerGold;
        PlayerGoldText.text = playerGold.ToString();
    }

    void GetPlayerRubi()
    {
        playerRubi = gameSession.playerRubi;
        PlayerRubiText.text = playerRubi.ToString();
    }

    void UpdateCurrentTime()
    {      
        bossEncounterTimer = FindFirstObjectByType<BossEncounterTimer>();
        if (bossEncounterTimer != null && bossEncounterTimer.isCountingDown)
        {
            countdownText.text = Mathf.CeilToInt(bossEncounterTimer.currentTime).ToString() + "s";
        }
        
    }
}
