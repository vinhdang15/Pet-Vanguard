using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBushByPlayer : MonoBehaviour
{
    Animator m_animator;
    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_animator.SetBool("hidePlayer", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_animator.SetBool("hidePlayer", false);
        }
    }
}
