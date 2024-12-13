using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScriptStartMap : MonoBehaviour
{

    private POSSystem posSystem;
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

    private bool canControl = true; // 컨트롤 가능 여부

    public void SetControlState(bool state)
    {
        canControl = state;
    }


    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();

        // Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        


        if (canControl) {
            CameraRotation();
        }
        

        // 움직임 입력 받기
        float moveX = Input.GetAxisRaw("Horizontal"); // 즉시 -1, 0, 1 값으로 변화
        float moveZ = Input.GetAxisRaw("Vertical"); 
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        rb.MovePosition(transform.position + move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed, isGrounded: " + isGrounded);
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void CameraRotation() {
        // 마우스 좌우 움직임에 따라 카메라 회전
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        // 마우스 위아래 움직임에 따라 카메라 회전
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        // 카메라 회전 각도 제한
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        // 좌우 카메라 회전
        transform.rotation = Quaternion.Euler(0, rotationX, 0);    
        // 위아래 카메라 회전
        playerCamera.transform.localRotation = Quaternion.Euler(-rotationY, 0, 0);
        // UI가 켜져있으면 불가능하면 카메라 화면회전 메서드 실행하지 않음
           
    } 

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
