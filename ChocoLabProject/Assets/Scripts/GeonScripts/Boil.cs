using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Boil : MonoBehaviour
{
    private PlayerScriptStartMap playerScript;
    // uiDocument 참조
    [SerializeField] private UIDocument uiDocument; 
    // tasteManager 참조
    [SerializeField] private Taste tasteManager;
    
    // UI Toolkit VisualElement 참조
    private VisualElement rootElement;
    private VisualElement boilUI;
    // UI 실행 여부
    private bool isUIActive = false;

    // 끓이기 상태 데이터 클래스
    public class BoilData {
        public bool isBoiling { get; set; }
        public float boilProgress { get; set; }
    }

    // 끓이기 상태 데이터 딕셔너리
    private Dictionary<int, BoilData> boilStates = new Dictionary<int, BoilData>();
    // 초콜릿 데이터 리스트
    public List<Taste.ChocolateData> chocolates => tasteManager.GetChocolates();

    // 끓인 초콜릿 수량 변수
    private int boiledDark = 0;
    private int boiledMilk = 0;
    private int boiledStrawberry = 0;
    private int boiledMatcha = 0;

    void Start()
    {   
        playerScript = GameObject.Find("Player").GetComponent<PlayerScriptStartMap>();
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        if (tasteManager == null || tasteManager.GetChocolates() == null) yield break;
        SetupUI();
    }

    private void SetupUI()
    {
        // UI 루트 엘리먼트 참조
        rootElement = uiDocument.rootVisualElement;
        // BoilUI 엘리먼트 참조
        boilUI = rootElement.Q<VisualElement>("BoilUI");
        
        // 끓이기 상태 데이터 초기화
        for (int i = 0; i < 4; i++)
        {
            boilStates[i] = new BoilData { isBoiling = false, boilProgress = 0f };
        }
            // 끓이기 UI 초기화
        boilUI.style.display = DisplayStyle.None;
        // 초콜릿 끓이기 UI에 필요한 데이터 설정
        SetupChocolateBoilUI("Dark", 0);
        SetupChocolateBoilUI("Milk", 1);
        SetupChocolateBoilUI("Strawberry", 2);
        SetupChocolateBoilUI("Matcha", 3);
    }

    // 초콜릿베이스 개수 출력
    private void SetupChocolateBoilUI(string chocolateType, int index)
    {
        var chocolateElement = boilUI.Q<VisualElement>(chocolateType);
        // 초콜릿 데이터가 없거나 인덱스가 범위를 벗어나면 종료
        if (chocolateElement == null || index >= chocolates.Count) return;

        // 현재 초콜릿 타입에 따른 맛과 양 출력할 라벨 참조 (UI Toolkit)
        var flavorLabel = chocolateElement.Q<Label>("FlavorLabel");
        var amountLabel = chocolateElement.Q<Label>("AmountLabel");
        // 끓이기 버튼 참조 (UI Toolkit)
        var boilButton = chocolateElement.Q<Button>("BoilButton");
        
        // 초콜릿 맛을 텍스트로 설정
        flavorLabel.text = chocolates[index].Flavor;
        // 초콜릿 수량을 텍스트로 설정
        amountLabel.text = $"수량: {chocolates[index].Amount}";
        // 끓이기 버튼 활성화 여부 설정
        
        // 현재 초콜릿 인덱스 저장
        int currentIndex = index;
        // 끓이기 버튼 클릭 시 끓이기 함수 실행
        boilButton.clicked += () => StartBoiling(currentIndex);
        // 끓이기 버튼 활성화 여부 설정 - 초콜릿 수량이 3개 이상이고 끓이기 상태가 아니면 활성화
        boilButton.SetEnabled(chocolates[currentIndex].Amount >= 3 && !boilStates[currentIndex].isBoiling);
    }

    // 끓이기 함수
    private void StartBoiling(int index)
    {       
        tasteManager.RemoveChocolates(index, 3); // 초콜릿 수량 3개 감소
        boilStates[index].isBoiling = true; // 끓이기 상태 시작 - Taste.cs에서 초콜릿 수량 감소
        StartCoroutine(BoilProcess(index)); // 끓이기 코루틴 실행
        RefreshUI(); // UI 갱신
    }

    // 끓이기 코루틴
    private IEnumerator BoilProcess(int index)
    {
        // 끓이기 진행 상태 표시 바 참조
        var progressBar = boilUI.Q<VisualElement>($"{chocolates[index].Flavor}")?.Q<ProgressBar>("BoilProgress");
        // 특정 초콜릿의 boilProgress가 100보다 작으면 반복
        while (boilStates[index].boilProgress < 100f)
        {
            boilStates[index].boilProgress += 1f; // boilProgress 1씩 증가
            progressBar.value = boilStates[index].boilProgress; // 끓이기 진행 상태 표시 바 갱신
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        }
        // 끓이기 완료
        switch(index)
        {
            case 0: boiledDark += 3; break; // 다크초콜릿 끓이기 완료되면 boiledDark 변수 3씩 증가 
            case 1: boiledMilk += 3; break; // 밀크초콜릿 끓이기 완료되면 boiledMilk 변수 3씩 증가 
            case 2: boiledStrawberry += 3; break; // 딸기초콜릿 끓이기 완료되면 boiledStrawberry 변수 3씩 증가 
            case 3: boiledMatcha += 3; break; // 말차초콜릿 끓이기 완료되면 boiledMatcha 변수 3씩 증가    
        }
        Debug.Log("boiledDark: " + boiledDark);
        Debug.Log("boiledMilk: " + boiledMilk);
        Debug.Log("boiledStrawberry: " + boiledStrawberry);
        Debug.Log("boiledMatcha: " + boiledMatcha);


        boilStates[index].isBoiling = false; // 끓이기 상태 종료
        boilStates[index].boilProgress = 0f; // boilProgress 초기화
    }

    public int GetBoiledChocolateCount(string flavor) {
        switch(flavor) {
            case "Dark": return boiledDark;
            case "Milk": return boiledMilk;
            case "Strawberry": return boiledStrawberry;
            case "Matcha": return boiledMatcha;
        } 
        return 0;
    }

    // 마우스키 누르면 끓이기 UI 토글 함수 실행
    void OnMouseDown()
    {
        if (!isUIActive) {
            OpenToggleUI();
        }
    }
    // 끓이기 UI 토글 함수
    private void OpenToggleUI() {  
        isUIActive = true;
        boilUI.style.display = DisplayStyle.Flex;
        RefreshUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerScript.SetControlState(false);
    }

    private void CloseToggleUI() {
        isUIActive = false;
        boilUI.style.display = DisplayStyle.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerScript.SetControlState(true);
    }

    // UI 갱신 함수
    private void RefreshUI()
    {
        // 초콜릿 데이터 리스트 수만큼 반복
        for (int i = 0; i < chocolates.Count; i++) {
            // 초콜릿 타입에 따라 끓이기 UI 엘리먼트 참조하기 위해 인덱스에 따른 초콜릿 타입 설정
            var chocolateType = i switch
            {
                0 => "Dark",
                1 => "Milk",
                2 => "Strawberry",
                3 => "Matcha",
            };
            // 초콜릿 타입에 따라 끓이기 UI 엘리먼트 참조
            var chocolateElement = boilUI.Q<VisualElement>(chocolateType);
            
            var amountLabel = chocolateElement.Q<Label>("AmountLabel");
            var flavorLabel = chocolateElement.Q<Label>("FlavorLabel");
            var boilButton = chocolateElement.Q<Button>("BoilButton");

            amountLabel.text = $"수량: {chocolates[i].Amount}";
            flavorLabel.text = chocolates[i].Flavor;
            boilButton.SetEnabled(chocolates[i].Amount >= 3 && !boilStates[i].isBoiling);
        }
    }

    void Update()
    {
        // isUIActive: UI가 현재 열려있는지 확인
        if (Input.GetKeyDown(KeyCode.Q) && isUIActive)
        {
            CloseToggleUI();
        }
    }
}