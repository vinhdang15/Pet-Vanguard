using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioSource pickedCoinAudioSoucre;
    GameSession gameSession;

    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            pickedCoinAudioSoucre.Play();
            gameObject.SetActive(false);
            //Destroy(gameObject);
            gameSession.TakeGem(collision.tag);
            
        }
    }
}
