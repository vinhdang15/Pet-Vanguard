using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public int playerHealthMax = 5;
    public int playerHealth;
    public int playerGold = 0;
    public int playerRubi = 0;
    public bool isGameOver = false;
    public GameObject[] npcPrefabs;
    public AudioSource playerAudioSource;

    public static bool isFristLaunch = true;

    void Awake()
    {
        int numberGameSession = FindObjectsOfType<GameSession>().Length;
        if (numberGameSession > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (isFristLaunch)
        {
            playerHealth = playerHealthMax;
            PlayerPrefs.DeleteAll();          
            playerAudioSource.loop = true;
            playerAudioSource.Play();
            isFristLaunch = false;
        }       
    }

    // To make sure playerHealth will show 0, when game overd
    public void ProcessPlayerTakeDamage()
    {
        if (playerHealth > 0) TakeDamage();
        if (playerHealth == 0)
        {
            isGameOver = true;
            GameOver();
        }
    }

    void TakeDamage()
    {
        playerHealth--;
    }
    public void TakeGem(string gemTag)
    {
        if (gemTag == "Gold") playerGold++;
        else if (gemTag == "Rubi") playerRubi++;
    }

    public void GameOver()
    {
        FindObjectOfType<LevelChanger>().FadeToGameOver();
    }

    public void ResetGameSession()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void LoadNPC()
    {
        if (PlayerPrefs.HasKey("NPC_ATK")) 
        {
            int npcType = PlayerPrefs.GetInt("NPC_ATK");
            InstantiateNPC(npcType);
        }
        if (PlayerPrefs.HasKey("NPC_DEF"))
        {
            int npcType = PlayerPrefs.GetInt("NPC_DEF");
            InstantiateNPC(npcType);
        }
    }

    void InstantiateNPC(int index)
    {
        Vector2 playerPosition = FindObjectOfType<PlayerMovement>().transform.position;
        GameObject npc = Instantiate(npcPrefabs[index], new Vector2(playerPosition.x, playerPosition.y), transform.rotation);
        npc.GetComponent<NPCMovement>().FollowState();
    }
}
