using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : MonoBehaviour
{
    [SerializeField] float m_LifeTime = 5f;

    void Awake()
    {
        Invoke("Kill", m_LifeTime);
    }

    void Kill()
    {
        Destroy(gameObject);
    }
}
