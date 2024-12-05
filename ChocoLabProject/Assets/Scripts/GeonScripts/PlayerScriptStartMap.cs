using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScriptStartMap : MonoBehaviour
{

    public Image crosshair;
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

    public float pickupRange = 3f;        // 물체를 집을 수 있는 거리
    public Transform holdPosition;         // 물체를 들고있을 위치
    private GameObject heldObject;         // 현재 들고있는 물체
    private Rigidbody heldObjectRb;       // 들고있는 물체의 Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
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

        // 물체 집기/놓기
        if (Input.GetMouseButtonDown(0))  // 마우스 좌클릭
        {
            if (heldObject == null)
            {
                // 물체 집기 - 화면 중앙에서 레이캐스트 발사
                RaycastHit pickupHit;
                Ray pickupRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                
                if (Physics.Raycast(pickupRay, out pickupHit, pickupRange))
                {
                    // Rigidbody가 있는 물체만 집을 수 있음
                    Rigidbody rb = pickupHit.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        heldObject = pickupHit.collider.gameObject;
                        heldObjectRb = rb;
                        heldObjectRb.useGravity = false;
                        heldObjectRb.isKinematic = true;
                        heldObject.transform.parent = holdPosition;
                        heldObject.transform.position = holdPosition.position;
                    }
                }
            }
            else
            {
                // 물체 놓기
                heldObject.transform.parent = null;
                heldObjectRb.useGravity = true;
                heldObjectRb.isKinematic = false;
                heldObject = null;
                heldObjectRb = null;
            }
        }

        // 크로스헤어 레이캐스트 체크
        Ray posRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit posHit;
        
        if (Physics.Raycast(posRay, out posHit, pickupRange))
        {
            // POS 시스템 체크
            POSSystem posSystem = posHit.collider.GetComponent<POSSystem>();
            if (posSystem != null)
            {
                // 크로스헤어가 POS에 닿았을 때
                if (Input.GetMouseButtonDown(0))  // 마우스 좌클릭
                {
                    posSystem.OpenPOSUI();
                }
            }
        }

        // ESC 키로 POS UI 닫기 (선택사항)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            POSSystem[] posSystems = FindObjectsOfType<POSSystem>();
            foreach (POSSystem pos in posSystems)
            {
                pos.ClosePOSUI();
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
