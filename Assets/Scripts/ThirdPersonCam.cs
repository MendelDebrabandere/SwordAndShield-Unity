using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform playerObj;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float rotationSpeed;

    private Vector3 m_ViewDir;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // rotate orientation
        m_ViewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = m_ViewDir.normalized;

        // rotate player object
        float horizontalInput = Input.GetAxis("MovementHorizontal");
        float verticalInput = Input.GetAxis("MovementVertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    public Vector3 GetViewDir()
    {
        return m_ViewDir;
    }
}
