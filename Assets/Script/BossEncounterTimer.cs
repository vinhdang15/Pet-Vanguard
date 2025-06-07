using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BossEncounterTimer : MonoBehaviour
{
    [SerializeField] float countdownTimer = 60f;
    [SerializeField] public float currentTime;
    public bool isCountingDown = false;
    [SerializeField] GameSession gameSession;
    [SerializeField] LevelChanger levelChanger;

    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        currentTime = countdownTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCountingDown)
        {
            currentTime -= Time.deltaTime;
        }
        if(currentTime < 0)
        {
            gameSession.GameOver();
        }
    }

    public void StartCountdown()
    {
        isCountingDown = true;
    }

    public void StopCountdown()
    {
        isCountingDown = false;
    }

    void GameOver()
    {
        gameSession.playerHealth = 0;
        gameSession.ProcessPlayerTakeDamage();
    }
}
