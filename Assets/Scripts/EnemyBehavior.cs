using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_StopRange = 2.7f;

    [Header("Attacking")]
    [SerializeField] private float m_AttackRange = 3f;
    [SerializeField] private float m_AggroRange = 20.0f;
    [SerializeField] private float m_MaxAttackCooldown = 2f;

    private float m_AttackCoolDownTimer = 0f;
    private bool m_AttackCoolDown = false;
    private Rigidbody m_RigidBody;
    private GameObject m_PlayerTarget;
    private SwordBehavior m_SwordBehavior;
    private bool m_IsAggro = false;
    private NavMeshAgent m_NavMeshAgent;

    private void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        m_PlayerTarget = player.gameObject;

        m_SwordBehavior = GetComponentsInChildren<SwordBehavior>()[0];

        m_RigidBody = GetComponent<Rigidbody>();
        m_RigidBody.freezeRotation = true;

        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.speed = m_MoveSpeed;
    }

    private void FixedUpdate()
    {
        m_RigidBody.velocity = Vector3.zero;
        if (!m_IsAggro)
        {
            m_NavMeshAgent.isStopped = true;
            if ((m_PlayerTarget.transform.position - gameObject.transform.position).sqrMagnitude <= m_AggroRange * m_AggroRange &&
                m_PlayerTarget.transform.position.y - gameObject.transform.position.y <= 3)
            {
                m_IsAggro = true;
            }
        }

        if (m_IsAggro)
        {
            m_NavMeshAgent.SetDestination(m_PlayerTarget.transform.position);
            m_NavMeshAgent.isStopped = false;

            if ((m_PlayerTarget.transform.position - gameObject.transform.position).sqrMagnitude <= m_AttackRange * m_AttackRange)
            {
                if (!m_AttackCoolDown)
                {
                    m_SwordBehavior.Attack();
                    m_AttackCoolDown = true;
                }
                else
                {
                    m_AttackCoolDownTimer += Time.deltaTime;
                    if (m_AttackCoolDownTimer >= m_MaxAttackCooldown)
                    {
                        m_AttackCoolDownTimer = 0f;
                        m_AttackCoolDown = false;
                    }
                }
            }

            if ((m_PlayerTarget.transform.position - gameObject.transform.position).sqrMagnitude <= m_StopRange * m_StopRange * 2f && m_RigidBody.velocity.sqrMagnitude >= Mathf.Pow(m_MoveSpeed * 0.5f, 2))
            {
                m_NavMeshAgent.isStopped = true;

                transform.forward = (m_PlayerTarget.transform.position - gameObject.transform.position).normalized;
            }
            if ((m_PlayerTarget.transform.position - gameObject.transform.position).sqrMagnitude <= m_StopRange * m_StopRange)
            {
                m_NavMeshAgent.isStopped = true;

                transform.forward = (m_PlayerTarget.transform.position - gameObject.transform.position).normalized;
            }
        }

        m_IsAggro = false;
    }
}
