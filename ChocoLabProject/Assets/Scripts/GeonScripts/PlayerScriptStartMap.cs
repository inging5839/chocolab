using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptStartMap : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    private bool isGrounded;

    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minY = -60f;
    public float maxY = 60f;
    public float rotationX = 0f;
    public float rotationY = 0f;
    private Rigidbody rb;
    public Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        transform.rotation = Quaternion.Euler(0, rotationX, 0);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(-rotationY, 0, 0);
        }

        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        rb.MovePosition(transform.position + move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed, isGrounded: " + isGrounded);
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log("Jump!");
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        Debug.Log("Grounded");
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        Debug.Log("Not Grounded");
    }
}
