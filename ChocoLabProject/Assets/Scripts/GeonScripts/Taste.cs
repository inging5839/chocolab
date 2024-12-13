using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Taste : MonoBehaviour
{
    [SerializeField] private PlayerScriptStartMap playerScript;
    [SerializeField] private UIDocument uiDocument;
    private VisualElement rootElement;
    private VisualElement tasteUI;
    private bool isUIActive = false;

    public class ChocolateData
    {
        public string Flavor { 
            get; 
            set;
        }
        public int Amount { 
            get; 
            set;
        }
        public bool CanAddMore => Amount < 5;
    }

    public List<ChocolateData> chocolates = new List<ChocolateData>();

    void Start()
    {
        InitializeChocolates();
        SetupUI();
    }

    private void InitializeChocolates()
    {
        // 초콜릿 데이터 추가
        chocolates.Add(new ChocolateData { 
            Flavor = "Dark",
            Amount = 0,
        });
        chocolates.Add(new ChocolateData { 
            Flavor = "Milk",
            Amount = 0,
        });
        chocolates.Add(new ChocolateData { 
            Flavor = "Strawberry",
            Amount = 0,
        });
        chocolates.Add(new ChocolateData { 
            Flavor = "Matcha",
            Amount = 0,
        });
    }

    private void SetupUI()
    {
        rootElement = uiDocument.rootVisualElement;
        tasteUI = rootElement.Q<VisualElement>("TasteUI");
            
        tasteUI.style.display = DisplayStyle.None;
        // 각 초콜릿 종류별로 UI 설정
        SetupChocolateUI("Dark", 0);
        SetupChocolateUI("Milk", 1);
        SetupChocolateUI("Strawberry", 2);
        SetupChocolateUI("Matcha", 3);
    }

    private void SetupChocolateUI(string chocolateType, int index)
    {
        var chocolateElement = tasteUI.Q<VisualElement>(chocolateType);
        if (chocolateElement != null)
        {
            var flavorLabel = chocolateElement.Q<Label>("FlavorLabel");
            var amountLabel = chocolateElement.Q<Label>("AmountLabel");
            var addButton = chocolateElement.Q<Button>("AddButton");
            var removeButton = chocolateElement.Q<Button>("RemoveButton");
                
            flavorLabel.text = chocolates[index].Flavor;
            amountLabel.text = $"수량: {chocolates[index].Amount}/5";
                
            addButton.SetEnabled(chocolates[index].CanAddMore);
            removeButton.SetEnabled(chocolates[index].Amount > 0);

            addButton.clicked += () => {
                AddChocolate(index);
                RefreshUI();
            };
            removeButton.clicked += () => {
                RemoveChocolate(index);
                RefreshUI();
            };
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < chocolates.Count; i++)
        {
            var chocolateType = i switch
            {
                0 => "Dark",
                1 => "Milk",
                2 => "Strawberry",
                3 => "Matcha",
                _ => throw new System.ArgumentOutOfRangeException()
            };

            var chocolateElement = tasteUI.Q<VisualElement>(chocolateType);
            
            var amountLabel = chocolateElement.Q<Label>("AmountLabel");
            var addButton = chocolateElement.Q<Button>("AddButton");
            var removeButton = chocolateElement.Q<Button>("RemoveButton");
                
            amountLabel.text = $"수량: {chocolates[i].Amount}/5";
            addButton.SetEnabled(chocolates[i].CanAddMore);
            removeButton.SetEnabled(chocolates[i].Amount > 0);
        }
    }

    public void AddChocolate(int index)
    {
        if (index >= 0 && index < chocolates.Count)
        {
            var chocolate = chocolates[index];
            if (chocolate.CanAddMore)
            {
                chocolate.Amount++;
            }
        }
    }

    private void RemoveChocolate(int index)
    {
        if (index >= 0 && index < chocolates.Count)
        {
            var chocolate = chocolates[index];
            if (chocolate.Amount > 0)
            {
                chocolate.Amount--;
            }
        }
    }

    public void RemoveChocolates(int index, int amount)
    {
        if (index >= 0 && index < chocolates.Count)
        {
            var chocolate = chocolates[index];
            if (chocolate.Amount >= amount)
            {
                chocolate.Amount -= amount;
                RefreshUI();
            }
        }
    }
    private void OpenToggleUI() {  
        isUIActive = true;
        tasteUI.style.display = DisplayStyle.Flex;
        RefreshUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerScript.SetControlState(false);
    }

    private void CloseToggleUI() {
        isUIActive = false;
        tasteUI.style.display = DisplayStyle.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerScript.SetControlState(true);
    }


    public List<ChocolateData> GetChocolates()
    {
        return chocolates;
    }

    void OnMouseDown() {
        if (!isUIActive) {
        OpenToggleUI();
        }
    }

    
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Q) && isUIActive)
        {
            CloseToggleUI();
        }
    }
}
