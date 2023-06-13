using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform m_ShieldTransform;
    [SerializeField]
    private Transform m_BlockTransform;
    [SerializeField]
    private bool m_IsFriendly;
    [SerializeField]
    private Transform m_Sword = null;


    [SerializeField]
    private Transform m_PlayerOrientation = null;

    private ThirdPersonCam m_Cam = null;

    private Vector3 m_StartBlockPosition;
    private Quaternion m_StartBlockRotation;
    public bool m_Blocking = false;

    void Start()
    {
        m_Cam = FindObjectOfType<ThirdPersonCam>();

        m_StartBlockPosition = m_ShieldTransform.localPosition;
        m_StartBlockRotation = m_ShieldTransform.localRotation;
    }

    void Update()
    {
        bool attackInput = false;

        if (m_IsFriendly)
            attackInput = Input.GetKey(KeyCode.Mouse1);

        if (attackInput)
            Block();
        else
            m_Blocking = false;

        // Check if blocking
        if (m_Sword)
        {
            SwordBehavior sword = m_Sword.gameObject.GetComponent<SwordBehavior>();
            if (sword != null)
            {
                if (sword.m_Attacking)
                    m_Blocking = false;
            }
        }


        if (m_Blocking)
        {
            m_ShieldTransform.localPosition = m_StartBlockPosition + m_BlockTransform.localPosition;
            m_ShieldTransform.localRotation = m_BlockTransform.localRotation;


            if (m_Cam != null && m_PlayerOrientation != null)
            {
                m_PlayerOrientation.forward = m_Cam.GetViewDir();
            }

        }
        else
        {
            m_ShieldTransform.localPosition = m_StartBlockPosition;
            m_ShieldTransform.localRotation = m_StartBlockRotation;
        }
    }

    public void Block()
    {
        m_Blocking = true;
   }
}
