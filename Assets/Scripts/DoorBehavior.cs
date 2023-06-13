using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] int m_EnemiesKilledForOpen = 0;

    [SerializeField]
    private AudioSource m_OpenDoorSound = null;

    private Health m_Health;
    private bool m_Destroying = false;

    void Start()
    {
        m_Health = FindObjectOfType<Health>();
    }

    void Update()
    {
        if (m_Health != null)
        {
            if (m_Health.GetDeaths() >= m_EnemiesKilledForOpen && m_Destroying == false)
            {
                if (m_OpenDoorSound)
                m_OpenDoorSound.Play();
                m_Destroying = true;
                Invoke("DestroyIt", 1f);
            }
        }
        else
        {
            m_Health = FindObjectOfType<Health>();
        }
    }

    void DestroyIt()
    {
        Destroy(gameObject);
    }
}

