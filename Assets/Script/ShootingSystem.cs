using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [Header("Click to choose use Animation Event")]
    [SerializeField] bool useAnimationEvent = false;
    [Header("sprite original gun direction: 1 for right, -1 for left")]
    public int spriteDirection;
    [Header("Fire Bullet Setup")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform gunPosition;
    [SerializeField] float ShootingDelay = 2f;
    [SerializeField] AudioSource bulletFiring;    
    GameSession gameSession;

    bool isShooting = false;
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        if (!useAnimationEvent)
        {
            StartCoroutine(Shoot());
        }      
    }

    IEnumerator Shoot()
    {
        while (true)
        { 
            if (isShooting)
            {
                Shooting();
            }
            yield return new WaitForSeconds(ShootingDelay);
        }        
    }

    public void SetShootingState(bool state)
    {
        isShooting = state;
    }

    // use this function for Animation Event && Coroutine
    public void Shooting()
    {
        if (gameSession.isGameOver == true) return;

        GameObject bullet = Instantiate(bulletPrefab, gunPosition.transform.position, transform.rotation);
        bulletFiring.Play();     
        //  use to detect sprite-gun-direction image, to adjust bullet direction
        Vector2 direction = new Vector2(transform.localScale.x * Mathf.Sign(spriteDirection), 0f);
        bullet.GetComponent<Bullet>().InitializeDirection(direction);
    }
}
