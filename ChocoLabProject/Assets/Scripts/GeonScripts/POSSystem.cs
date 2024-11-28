using UnityEngine;
using UnityEngine.UIElements;

public class POSSystem : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private bool isUIActive = false;
    private VisualElement posScreen;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument가 할당되지 않았습니다.");
            return;
        }

        posScreen = uiDocument.rootVisualElement.Q<VisualElement>("PosUI");
        
        if (posScreen != null)
        {
            posScreen.style.display = DisplayStyle.None;
            isUIActive = false;
        }
    }

    public void OpenPOSUI()
    {
        if (posScreen == null) return;
        
        if (isUIActive) return;

        isUIActive = true;
        posScreen.style.display = DisplayStyle.Flex;
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void ClosePOSUI()
    {
        if (posScreen == null) return;

        if (!isUIActive) return;

        isUIActive = false;
        posScreen.style.display = DisplayStyle.None;
        
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    public bool IsUIOpen()
    {
        return isUIActive;
    }

    void Update()
    {
        
    }
}
