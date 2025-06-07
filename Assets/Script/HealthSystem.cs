using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] int maxHealth = 3;
    [SerializeField] bool BounceBackWhenTakeDamage;
    [SerializeField] float HorizontalBounceBack = 100f;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamageByBullet(int damage)
    {
        if (currentHealth > 0) 
        { 
            currentHealth -= damage;
            healthSlider.value = currentHealth;
        }
        if (currentHealth == 0)
        {         
            Dead();
        }
        animator.SetTrigger("triggerTakeBulletDamage");
        
        float x = gameObject.GetComponent<Transform>().localScale.x;
        if (BounceBackWhenTakeDamage)
        {
            rb.velocity += new Vector2(HorizontalBounceBack * x, 0f);
        }
    }
    public void Dead()
    {
        BossMovement bossMovement;
        if (TryGetComponent(out bossMovement))
        {
            GameSession.isFristLaunch = true;
            FindObjectOfType<GameSession>().GameOver();
        }        
        Destroy(gameObject);
    }
}
