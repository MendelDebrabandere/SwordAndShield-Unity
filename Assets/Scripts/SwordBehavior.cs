using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform m_SwordTransform;
    [SerializeField]
    private Transform m_BeginAnimTransform;
    [SerializeField]
    private Transform m_EndAnimTransform;
    [SerializeField]
    private int m_Damage = 5;
    [SerializeField]
    private bool m_IsFriendly;
    [SerializeField]
    private Transform m_Shield = null;

    private PlayerMovement m_PlayerMovement = null;

    [SerializeField]
    private Transform m_PlayerOrientation = null;

    private ThirdPersonCam m_Cam = null;

    [SerializeField] GameObject m_BloodFX = null;
    [SerializeField] GameObject m_EmpowerFX = null;


    private Vector3 m_StartSwordTransformPosition;
    private Quaternion m_StartSwordTransformRotation;
    public bool m_Attacking = false;
    static private float m_MaxAttackTime = 0.45f;
    private float m_AttackingTime = -.1f;
    float m_PlayerAttackCooldown = 0f;
    const float m_MaxPlayerAttackCooldown = 1f;

    const float m_MaxEmpoweredTimer = 1.2f;
    private float m_EmpoweredTimer = 0f;

    const string ENEMY_TAG = "Enemy";
    const string FRIENDLY_TAG = "Friendly";
    const string SHIELD_TAG = "Shield";


    [SerializeField]
    private Color m_FlickerColor = Color.yellow;

    private Color m_StartColor;
    [SerializeField]
    private Material m_AttachedMaterial = null;

    [SerializeField] private AudioSource m_AttackSound = null;

    [SerializeField]
    private AudioSource m_BlockEffect = null;


    const string COLOR_PARAMETER = "_Color";


    void Start()
    {
        m_Cam = FindObjectOfType<ThirdPersonCam>();

        m_StartSwordTransformPosition = m_SwordTransform.localPosition;
        m_StartSwordTransformRotation = m_SwordTransform.localRotation;

        m_PlayerMovement = FindObjectOfType<PlayerMovement>();

        Renderer renderer = transform.GetComponentInChildren<Renderer>();
        if (renderer)
        {
            m_AttachedMaterial = renderer.material;

            if (m_AttachedMaterial)
                m_StartColor = m_AttachedMaterial.GetColor(COLOR_PARAMETER);
        }

    }
    void Update()
    {
        bool attackInput = false;

        if (m_IsFriendly)
        {
            attackInput = Input.GetKey(KeyCode.Mouse0);
        }

        if (attackInput)
        {
            if (m_PlayerAttackCooldown <= 0f)
            { 
                m_PlayerAttackCooldown = m_MaxPlayerAttackCooldown;
                Attack();
            }
        }

        if (m_Shield)
        {
            // Check if blocking
            ShieldBehavior shield = m_Shield.gameObject.GetComponent<ShieldBehavior>();
            if (shield != null)
            {
                if (shield.m_Blocking)
                {
                    m_Attacking = false;
                }
            }
        }

        if (m_Attacking)
        {
            if (m_IsFriendly)
            {
                if (m_Cam != null && m_PlayerOrientation != null)
                {
                    m_PlayerOrientation.forward = m_Cam.GetViewDir();
                }
            }

            m_AttackingTime += Time.deltaTime;

            m_SwordTransform.localPosition = m_StartSwordTransformPosition + Vector3.Lerp(m_BeginAnimTransform.localPosition, m_EndAnimTransform.localPosition, m_AttackingTime / m_MaxAttackTime);
            m_SwordTransform.localRotation = Quaternion.Lerp(m_BeginAnimTransform.localRotation, m_EndAnimTransform.localRotation, m_AttackingTime / m_MaxAttackTime);

            if (m_AttackingTime > m_MaxAttackTime)
            {
                StopAttack();
            }

        }

        if (m_EmpoweredTimer >= 0f)
        {
            m_EmpoweredTimer -= Time.deltaTime;
        }

        if (m_PlayerAttackCooldown > 0f)
            m_PlayerAttackCooldown -= Time.deltaTime;
    }

    public void Attack()
    {
        m_Attacking = true;

        if (m_Shield)
        {
            // Check if blocking
            ShieldBehavior shield = m_Shield.gameObject.GetComponent<ShieldBehavior>();
            if (shield != null)
            {
                if (!shield.m_Blocking)
                {
                    if (m_AttackSound)
                        m_AttackSound.Play();
                }
            }
        }
        else
        {
            if (m_AttackSound)
                m_AttackSound.Play();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (m_Attacking)
        {
            //make sure we only hit friendly or enemies 
            if (other.tag == ENEMY_TAG && transform.parent.tag == ENEMY_TAG)
                return;
            if (other.tag == FRIENDLY_TAG && transform.parent.tag == FRIENDLY_TAG)
                return;
            if (other.tag == SHIELD_TAG)
            {
                ShieldBehavior shield = other.GetComponent<ShieldBehavior>();
                if (shield != null)
                {
                    if (shield.m_Blocking)
                    {
                        StopAttack();
                        EmpowerPlayerSword();
                        if (m_BlockEffect)
                            m_BlockEffect.Play();
                        return;
                    }
                }
            }


            Health otherHealth = other.GetComponent<Health>();

            if (otherHealth != null)
            {
                if (m_EmpoweredTimer <= 0f)
                {
                    otherHealth.Damage(m_Damage);
                    if (m_BloodFX)
                        Instantiate(m_BloodFX, transform.position + transform.up * 0.5f, transform.rotation);
                }
                else
                {
                    otherHealth.Damage(m_Damage * 2);
                    if (m_BloodFX)
                    {
                        Instantiate(m_BloodFX, transform.position + transform.up * 0.2f, transform.rotation);
                        Instantiate(m_BloodFX, transform.position + transform.up * 0.5f, transform.rotation);
                        Instantiate(m_BloodFX, transform.position + transform.up * 0.7f, transform.rotation);
                    }
                }
            }
        }
    }

    private void StopAttack()
    {
        m_AttackingTime = 0f;
        m_Attacking = false;
        m_SwordTransform.localPosition = m_StartSwordTransformPosition;
        m_SwordTransform.localRotation = m_StartSwordTransformRotation;
    }

    private void EmpowerPlayerSword()
    {
        if(m_PlayerMovement)
        {
            m_PlayerMovement.gameObject.GetComponentInChildren<SwordBehavior>().Empower();
        }
    }

    public void Empower()
    {
        if (m_AttachedMaterial)
        {
            m_AttachedMaterial.SetColor(COLOR_PARAMETER, m_FlickerColor);
            Invoke(RESET_COLOR_METHOD, m_MaxEmpoweredTimer);
        }

        m_EmpoweredTimer = m_MaxEmpoweredTimer;
        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = new Vector3(-90, 0, 0);
        Instantiate(m_EmpowerFX, transform.position, rot, transform);
    }

    const string RESET_COLOR_METHOD = "ResetColor";
    void ResetColor()
    {
        if (!m_AttachedMaterial)
            return;

        m_AttachedMaterial.SetColor(COLOR_PARAMETER, m_StartColor);
    }

    void OnDestroy()
    {
        if (m_AttachedMaterial != null)
            return;
        //since we created a new material in the start, we should clean it up 
        Destroy(m_AttachedMaterial);
    }
}
