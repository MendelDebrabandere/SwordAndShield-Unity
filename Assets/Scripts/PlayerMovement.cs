using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private Transform m_Orientation;
    [SerializeField] private float m_GroundDrag;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_AddedGravity;
    [SerializeField] private float m_JumpCooldown;
    [SerializeField] private float m_AirMultiplier;
    private bool m_ReadyToJump = true;

    [Header("Keybinds")]
    public KeyCode m_JumpKey = KeyCode.Space;

    [Header("GroundCheck")]
    [SerializeField] private LayerMask m_WhatIsGround;
    private bool m_Grounded;

    [Header("Slope Handling")]
    [SerializeField] private float m_MaxSlopeAngle;
    private RaycastHit m_SlopeHit;
    private bool m_ExitingSlope = false;

    private float m_HorizontalInput;
    private float m_VerticalInput;

    private Vector3 m_MoveDirection;

    Rigidbody m_Rb;

    private void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        m_Grounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, 1.2f, m_WhatIsGround);

        RegisterInput();
        SpeedControl();

        // handle drag
        if (m_Grounded)
            m_Rb.drag = m_GroundDrag;
        else
            m_Rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void RegisterInput()
    {
        m_HorizontalInput = Input.GetAxisRaw("MovementHorizontal");
        m_VerticalInput = Input.GetAxisRaw("MovementVertical");

        // Check for jump
        if (Input.GetKey(m_JumpKey) && m_ReadyToJump && m_Grounded)
        {
            m_ReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), m_JumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        m_MoveDirection = m_Orientation.forward * m_VerticalInput + m_Orientation.right * m_HorizontalInput;

        // on slope
        if (OnSlope() && !m_ExitingSlope)
        {
            m_Rb.AddForce(GetSlopeMoveDirection() * m_MoveSpeed * 20f, ForceMode.Force);

            //if (rb.velocity.y > 0)
            m_Rb.AddForce( -m_SlopeHit.normal * 80f, ForceMode.Force);
        }

        // on ground
        if (m_Grounded)
            m_Rb.AddForce(m_MoveDirection * m_MoveSpeed * 10f, ForceMode.Force);

        // in air
        else
        {
            m_Rb.AddForce(m_MoveDirection * m_MoveSpeed * 10f * m_AirMultiplier, ForceMode.Force);
            m_Rb.AddForce(m_Orientation.up * -1 * m_AddedGravity, ForceMode.Force);
        }

        // turn graviry off while on slope
        m_Rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !m_ExitingSlope)
        {
            if (m_Rb.velocity.magnitude > m_MoveSpeed)
                m_Rb.velocity = m_Rb.velocity.normalized * m_MoveSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(m_Rb.velocity.x, 0f, m_Rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > m_MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * m_MoveSpeed;
                m_Rb.velocity = new Vector3(limitedVel.x, m_Rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        m_ExitingSlope = true;

        // reset y velocity
        m_Rb.velocity = new Vector3(m_Rb.velocity.x, 0f, m_Rb.velocity.z);

        m_Rb.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        m_ReadyToJump = true;

        m_ExitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, out m_SlopeHit, 1.5f))
        {
            float angle = Vector3.Angle(Vector3.up, m_SlopeHit.normal);
            return Mathf.Abs(angle) < m_MaxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(m_MoveDirection, m_SlopeHit.normal).normalized;
    }
}
