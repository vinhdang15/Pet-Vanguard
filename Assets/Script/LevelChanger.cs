using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    Animator animator;
    int currentIndexScene;
    [SerializeField] GameSession gameSession;
    [SerializeField] TextMeshProUGUI endGameState;

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        animator = GetComponent<Animator>();
        currentIndexScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void FadeToLevel()
    {
        if (gameSession != null)
        {
            gameSession.playerAudioSource.Stop();
        }  
        animator.SetTrigger("isFadeOut");
    }

    public void FadeToGameOver()
    {
        gameSession.playerAudioSource.Stop();
        animator.SetTrigger("isGameOver");
    }

    //Animation Event
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(currentIndexScene + 1);
    }

    //Animation Event
    public void ShowGameOverPanel()
    {
        gameObject.GetComponentInChildren<Transform>().Find("Canvas - Level Changer/GameOver").gameObject.SetActive(true);
        if (!gameSession.isGameOver) endGameState.text = "YOU WIN";
        else endGameState.text = "GAME OVER";

    }
    
    //Button Event
    public void PlayAgain()
    {
        gameSession.ResetGameSession();
    }
}
