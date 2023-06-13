using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    static int m_Deaths = 0;

    [SerializeField] int m_StartHealth = 10;

    [SerializeField] private Color m_FlickerColor = Color.red;
    private float m_FlickerDuration = 0.1f;

    private Color m_StartColor;
    private Material m_AttachedMaterial;

    const string COLOR_PARAMETER = "_Color";

    private int m_CurrentHealth = 0;

    void Awake()
    {
        m_CurrentHealth = m_StartHealth;
    }

    private void Start()
    {
        Renderer renderer = transform.GetComponentInChildren<Renderer>();
        if (renderer)
        {
            //This will actually behind the scenes create a new instance of a material, use 
            //renderer.sharedmaterial to get the actual material use(be careful as this will change it
            //for ALL objects using that material) 
            m_AttachedMaterial = renderer.material;

            if (m_AttachedMaterial)
                m_StartColor = m_AttachedMaterial.GetColor(COLOR_PARAMETER);
        }
    }

    public void Damage(int amount)
    {
        m_CurrentHealth -= amount;

        if (m_AttachedMaterial)
        {
            m_AttachedMaterial.SetColor(COLOR_PARAMETER, m_FlickerColor);
            Invoke(RESET_COLOR_METHOD, m_FlickerDuration);
        }

        if (m_CurrentHealth <= 0)
            if (gameObject.GetComponent<PlayerMovement>() == null)
            {
                ++m_Deaths;
                Kill();
            }
    }

    const string RESET_COLOR_METHOD = "ResetColor";
    void ResetColor()
    {
        if (!m_AttachedMaterial)
            return;

        m_AttachedMaterial.SetColor(COLOR_PARAMETER, m_StartColor);

    }

    public void Heal(int amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth >= m_StartHealth)
        {
            m_CurrentHealth = m_StartHealth;
        }

    }


    void Kill()
    {
        Destroy(gameObject);
    }

    public float HealthPercentage
    {
        get
        {
            return ((float)m_CurrentHealth) / m_StartHealth;
        }
    }

    public int GetDeaths()
    {
        return m_Deaths;
    }

    public void ResetDeaths()
    {
        m_Deaths = 0;
    }

    void OnDestroy()
    {
        if (m_AttachedMaterial == null)
            return;
        //since we created a new material in the start, we should clean it up 
        Destroy(m_AttachedMaterial);
    }
}

