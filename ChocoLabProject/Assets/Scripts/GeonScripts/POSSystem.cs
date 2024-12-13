using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class POSSystem : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Taste tasteSystem;
    private bool isUIActive = false;
    private VisualElement posScreen;
    private ListView chocolateListView;

    void Start()
    {
        posScreen = uiDocument.rootVisualElement.Q<VisualElement>("PosUI");
            // UI 요소를 화면에서 완전히 숨김
        posScreen.style.display = DisplayStyle.None;
        // UI 활성 여부 false로 설정
        isUIActive = false;
            
        
    }

    public void TogglePOSUI()
    {        
        isUIActive = !isUIActive;
        posScreen.style.display = isUIActive ? DisplayStyle.Flex : DisplayStyle.None;
        


        if (isUIActive) {
        // 마우스 커서 잠금 해제
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // 마우스 커서 표시
        UnityEngine.Cursor.visible = true;
        } else {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isUIActive)
        {
            TogglePOSUI();
        }
    }

    void OnMouseDown() {
        TogglePOSUI();
    }
}
