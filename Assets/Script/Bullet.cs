using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    [SerializeField] float BulletSpeed = 10f;
    [SerializeField] int bulletDamge = 1;
    Rigidbody2D m_RigidBody2D;
    float bulletMoveDirect;
    bool hasDealDamage = false;
    Vector2 moveDirection;
    GameSession gameSession;

    public void InitializeDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        m_RigidBody2D = GetComponent<Rigidbody2D>();      

        m_RigidBody2D.velocity = moveDirection * BulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Flatform"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player") && !hasDealDamage)
        {
            gameSession.ProcessPlayerTakeDamage();
            hasDealDamage = true;
            Destroy(gameObject);         
            return;
        }
        if (collision.CompareTag("Enemy") && !hasDealDamage)
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamageByBullet(bulletDamge);
            hasDealDamage = true;
            Destroy(gameObject);
            return;
        }
    }
}
