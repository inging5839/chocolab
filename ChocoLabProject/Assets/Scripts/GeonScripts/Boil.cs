using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Boil : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Taste tasteManager;
    
    private VisualElement rootElement;
    private VisualElement boilUI;
    private bool isUIActive = false;

    private const int MAX_SIMULTANEOUS_BOIL = 3;
    private int currentBoilingCount = 0;

    public class BoilData {
        public bool isBoiling { get; set; }
        public float boilProgress { get; set; }
    }

    private Dictionary<int, BoilData> boilStates = new Dictionary<int, BoilData>();
    
    public List<Taste.ChocolateData> chocolates => tasteManager.GetChocolates();

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        
        if (tasteManager == null)
        {
            Debug.LogError("TasteManager is not assigned!");
            yield break;
        }
        
        var chocolatesList = tasteManager.GetChocolates();
        if (chocolatesList == null)
        {
            Debug.LogError("GetChocolates() returned null!");
            yield break;
        }
        
        Debug.Log($"Chocolates count: {chocolatesList.Count}");
        foreach (var chocolate in chocolatesList)
        {
            if (chocolate == null)
            {
                Debug.LogError("Found null chocolate in the list!");
                continue;
            }
            Debug.Log($"Chocolate - Flavor: {chocolate.Flavor}, Amount: {chocolate.Amount}");
        }
        
        SetupUI();
    }

    // private void InitializeBoilStates()
    // {
        
    // }

    private void SetupUI()
    {
        rootElement = uiDocument.rootVisualElement;
        boilUI = rootElement.Q<VisualElement>("BoilUI");
        
        for (int i = 0; i < 4; i++)
        {
            boilStates[i] = new BoilData { isBoiling = false, boilProgress = 0f };
        }

        if (boilUI != null)
        {
            boilUI.style.display = DisplayStyle.None;
            SetupChocolateBoilUI("Dark", 0);
            SetupChocolateBoilUI("Milk", 1);
            SetupChocolateBoilUI("Strawberry", 2);
            SetupChocolateBoilUI("Matcha", 3);
        }
    }

    private void SetupChocolateBoilUI(string chocolateType, int index)
    {
        Debug.Log($"Setting up UI for {chocolateType} at index {index}");
        
        var chocolateElement = boilUI.Q<VisualElement>(chocolateType);
        if (chocolateElement == null)
        {   
            Debug.LogError($"chocolateElement not found for {chocolateType}");
            return;
        }

        var flavorLabel = chocolateElement.Q<Label>("FlavorLabel");
        var amountLabel = chocolateElement.Q<Label>("AmountLabel");
        var boilButton = chocolateElement.Q<Button>("BoilButton");
        var progressBar = chocolateElement.Q<ProgressBar>("BoilProgress");

        
        // chocolates 리스트 범위 체크
        if (index >= chocolates.Count)
        {
            Debug.LogError($"Index {index} is out of range for chocolates list (size: {chocolates.Count})");
            return;
        }

        Debug.Log($"Setting up UI for {chocolateType} - Flavor: {chocolates[index].Flavor}, Amount: {chocolates[index].Amount}");
        
        flavorLabel.text = chocolates[index].Flavor;
        amountLabel.text = $"수량: {chocolates[index].Amount}";
        
        if (boilButton != null)
        {
            int currentIndex = index;
            boilButton.clicked += () => StartBoiling(currentIndex);
            
            Debug.Log($"Chocolate {chocolateType} - Amount: {chocolates[currentIndex].Amount}, IsBoiling: {boilStates[currentIndex].isBoiling}");
            
            boilButton.SetEnabled(chocolates[currentIndex].Amount >= 3 && !boilStates[currentIndex].isBoiling);
            Debug.Log($"Button enabled state for {chocolateType}: {boilButton.enabledSelf}");
        }
        
        RefreshUI();
    }

    private void StartBoiling(int index)
    {
        Debug.Log($"StartBoiling called for index: {index}");
        if (chocolates[index].Amount >= 3 && !boilStates[index].isBoiling)
        {
            tasteManager.RemoveChocolates(index, 3);
            
            boilStates[index].isBoiling = true;
            StartCoroutine(BoilProcess(index));
            
            RefreshUI();
        }
    }

    private IEnumerator BoilProcess(int index)
    {
        var progressBar = boilUI.Q<VisualElement>($"{chocolates[index].Flavor}")?.Q<ProgressBar>("BoilProgress");
        
        while (boilStates[index].boilProgress < 100f)
        {
            boilStates[index].boilProgress += 1f;
            if (progressBar != null)
            {
                progressBar.value = boilStates[index].boilProgress;
            }
            yield return new WaitForSeconds(0.1f);
        }

        boilStates[index].isBoiling = false;
        boilStates[index].boilProgress = 0f;
    }

    void OnMouseDown()
    {
        ToggleUI();
    }

    private void ToggleUI()
    {
        if (boilUI == null) return;

        isUIActive = !isUIActive;
        boilUI.style.display = isUIActive ? DisplayStyle.Flex : DisplayStyle.None;

        if (isUIActive)
        {
            RefreshUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void RefreshUI()
    {
        Debug.Log($"RefreshUI called. Chocolates count: {chocolates.Count}");
        
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

            Debug.Log($"Processing chocolate type: {chocolateType}, index: {i}");
            
            var chocolateElement = boilUI.Q<VisualElement>(chocolateType);
            Debug.Log($"Found chocolateElement for {chocolateType}: {chocolateElement != null}");
            
            if (chocolateElement != null)
            {
                var amountLabel = chocolateElement.Q<Label>("AmountLabel");
                var flavorLabel = chocolateElement.Q<Label>("FlavorLabel");
                var boilButton = chocolateElement.Q<Button>("BoilButton");

                if (amountLabel != null)
                {
                    amountLabel.text = $"수량: {chocolates[i].Amount}";
                }

                if (flavorLabel != null)
                {
                    flavorLabel.text = chocolates[i].Flavor;
                }

                if (boilButton != null)
                {
                    boilButton.SetEnabled(chocolates[i].Amount >= 3 && !boilStates[i].isBoiling);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isUIActive)
        {
            ToggleUI();
        }
    }
}
