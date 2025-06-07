using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveByPlayer : MonoBehaviour
{
    [SerializeField] LevelChanger levelChanger;
    [SerializeField] float delayToDestroy = 0.5f;
    [SerializeField] float delayToChangeSecens = 1f;
    PolygonCollider2D m_polyCollider;
    Animator m_animator;

    private void Start()
    {
        m_polyCollider = GetComponent<PolygonCollider2D>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (m_polyCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            m_animator.SetTrigger("hitPlayer");
            if (gameObject.name.Contains("TempSlab"))
            {
                Invoke(nameof(DestroyTempSlab), delayToDestroy);
            }
            if (gameObject.name.Contains("CheckPoint"))
            {
                Invoke(nameof(LoadNextScene), delayToChangeSecens);
            }
        }
    }

    public void LoadNextScene()
    {
        levelChanger.FadeToLevel();
    }

    private void DestroyTempSlab()
    {
        Destroy(gameObject);
    }
}
