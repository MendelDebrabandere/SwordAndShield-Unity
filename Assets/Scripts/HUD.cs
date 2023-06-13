using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{

    [SerializeField] Image m_HealthBar = null;
    [SerializeField] GameObject m_BackGround = null;
    [SerializeField] GameObject m_WinBackGround = null;

    private Health m_PlayerHealth = null;
    private bool m_GameOver = false;
    private bool m_Win = false;

    private Health m_Health;

    void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();

        m_BackGround.SetActive(false);
        m_WinBackGround.SetActive(false);

        if (player)
        {
            m_PlayerHealth = player.GetComponent<Health>();
        }

        m_Health = FindObjectOfType<Health>();
    }

    void Update()
    {
        SyncData();

        if (m_Health == null)
        {
            m_Health = FindObjectOfType<Health>();
        }
        if (!m_Win && !m_GameOver)
        {
            AudioListener.pause = false;
        }
    }    

    private void SyncData()
    {
        if (m_HealthBar && m_PlayerHealth)
        {
            m_HealthBar.transform.localScale = new Vector3(m_PlayerHealth.HealthPercentage, 1, 1);

            if (m_PlayerHealth.HealthPercentage <= 0.001f)
            {
                m_GameOver = true;
            }
        }

        if (m_Win)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_WinBackGround.SetActive(true);
            AudioListener.pause = true;
        }

        if (m_GameOver )
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_BackGround.SetActive(true);
            AudioListener.pause = true;
        }

    }

    public void RestartButton()
    {
        SceneManager.LoadScene("MainLevel");
        AudioListener.pause = true;
        m_Health.ResetDeaths();
    }

    public void Win()
    {
        m_Win = true;
    }    
}
