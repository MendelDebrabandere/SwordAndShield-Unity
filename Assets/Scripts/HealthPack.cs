using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] int m_HealAmount = 30;
    [SerializeField] int m_SpinSpeed = 1000000;

    [SerializeField]
    private AudioSource m_PickupSound = null;

    [SerializeField]
    private GameObject m_Visuals = null;

    bool m_Destroying = false;

    float m_SineAngle = 0f;

    private GameObject m_Player = null;

    void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        m_Player = player.gameObject;
    }

    void Update()
    {
        m_SineAngle += Time.deltaTime * 1.5f;

        transform.Translate(0, Mathf.Sin(m_SineAngle) / 600f, 0);

        transform.Rotate(0f, m_SpinSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_Destroying)
        {
            if (m_Player)
            {
                if (other.GetComponentInParent<PlayerMovement>())
                {
                    Health playerHealth = m_Player.GetComponent<Health>();
                    if (playerHealth)
                    {
                        playerHealth.Heal(m_HealAmount);
                        if (m_PickupSound)
                            m_PickupSound.Play();
                        m_Destroying = true;
                        m_Visuals.SetActive(false);
                        Invoke("DestroyIt", 1f);
                    }
                }
            }
        }
    }

    void DestroyIt()
    {
        Destroy(gameObject);
    }
}
