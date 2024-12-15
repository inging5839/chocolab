using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using Cursor = UnityEngine.Cursor;


public class Shape : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerScriptStartMap playerScript;
    private VisualElement rootElement;
    private VisualElement shapeUI;
    private VisualElement draggedElement;
    private Vector2 startPosition;
    private bool isDragging = false;
    private bool isUIActive = false;
    void Start()
    {
        SetupUI();
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && isUIActive) {
            CloseToggleUI();
        }
    }

    void OnMouseDown()
    {
        if (!isUIActive) {
            OpenToggleUI();
        }
    }

    private void SetupUI()
    {
        rootElement = uiDocument.rootVisualElement;
        shapeUI = rootElement.Q<VisualElement>("ShapeUI");
        shapeUI.style.display = DisplayStyle.None;
       
        SetupDraggableChocolates();
        SetupMolds();
    }

    // 드래그 가능한 초콜릿 설정 - 녹아있는 초콜릿
    private void SetupDraggableChocolates()
    {
        // 녹아있는 초콜릿 종류
        var chocolateTypes = new[] { "Dark", "Milk", "Strawberry", "Matcha" };
        // 녹아있는 초콜릿 영역
        var chocolateArea = rootElement.Q<VisualElement>("MeltedChocolateArea");
        
        foreach (var type in chocolateTypes)
        {
            // 녹아있는 초콜릿 영역에서 초콜릿 종류 찾기
            var chocolate = chocolateArea.Q<VisualElement>($"{type}Chocolate");
            // 드래그 가능하게 설정
            MakeDraggable(chocolate);
        }
    }

    // 드래그 가능한 초콜릿 설정 - 모양 영역
    private void SetupMolds()
    {
        var chocolateTypes = new[] { "Kisses", "Pepero", "Heart", "Star" };
        // 모양 영역 컨테이너 설정
        var moldContainer = rootElement.Q<VisualElement>("ChocolateMoldArea");

        // 모양 영역 종류 찾기
        foreach (var type in chocolateTypes)
        {
            // 모양영역 설정
            var mold = moldContainer.Q<VisualElement>($"{type}");
            // 모양영역 클래스 설정
            mold.AddToClassList("mold-slot");
            mold.AddToClassList($"shape-{type.ToLower()}");
            // 모양 영역 드랍 가능하게 설정
            MakeDroppable(mold);
        }
    }

    private void MakeDraggable(VisualElement element)
    {
        element.RegisterCallback<MouseDownEvent>(evt =>
        {
            // 드래그 중이 아니라면!
            if (!isDragging)
            {
                // 드래그 중인 요소 설정
                draggedElement = element;
                // 드래그 시작 위치 설정
                startPosition = evt.mousePosition;
                // 드래그 중 상태 설정
                isDragging = true;
                // 드래그 중 클래스 추가
                element.AddToClassList("dragging");
                // 이벤트 전파 중지
                evt.StopPropagation();
            }
        });

        // 드래그 중일 때 이벤트 처리
        element.RegisterCallback<MouseMoveEvent>(evt =>
        {
            // 드래그 중이고 드래그 중인 요소가 현재 요소라면!
            if (isDragging && draggedElement == element)
            {
                // 드래그 중인 요소의 위치 설정
                Vector2 delta = evt.mousePosition - startPosition;
                element.style.translate = new Translate(delta.x, delta.y, 0);
                // 이벤트 전파 중지
                evt.StopPropagation();
            }
        });

        // 드래그 종료 이벤트 처리
        element.RegisterCallback<MouseUpEvent>(evt =>
        {
            // 드래그 중이고 드래그 중인 요소가 현재 요소라면!
            if (isDragging && draggedElement == element)
            {
                // 드래그 중 상태 설정
                isDragging = false;
                // 드래그 중 클래스 제거
                draggedElement.RemoveFromClassList("dragging");
                // 드래그 종료 위치 처리
                CheckDropZone(evt.mousePosition);
                // 드래그 종료 위치 초기화
                ResetElementPosition(element);
                // 드래그 중인 요소 초기화
                draggedElement = null;
                // 이벤트 전파 중지
                evt.StopPropagation();
            }
        });
    }

    // 드랍 가능한 영역 설정
    private void MakeDroppable(VisualElement mold)
    {
        // 마우스 진입 이벤트 처리
        mold.RegisterCallback<MouseEnterEvent>(evt =>
        {
            // 드래그 중이라면!
            if (isDragging)
            {
                // 드랍 가능한 영역 강조 클래스 추가
                mold.AddToClassList("highlight");
            }
        });

        mold.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            // 드랍 가능한 영역 강조 클래스 제거
            mold.RemoveFromClassList("highlight");
        });
    }

    private void CheckDropZone(Vector2 position)
    {
        // 드랍 가능한 영역 찾기
        var molds = rootElement.Query<VisualElement>("ChocolateMold").ToList();
        foreach (var mold in molds)
        {
            // 드랍 가능한 영역 위에 있는지 확인
            if (IsPositionOverElement(position, mold))
            {
                // 드랍 처리
                OnChocolateDropped(mold, draggedElement.name);
                break;
            }
        }
    }

    private bool IsPositionOverElement(Vector2 position, VisualElement element)
    {
        // 드랍 가능한 영역의 경계 영역 찾기
        var rect = element.worldBound;
        // 드랍 가능한 영역의 경계 영역에 위치가 포함되어 있는지 확인
        return rect.Contains(position);
    }

    private void ResetElementPosition(VisualElement element)
    {
        // 드래그 중인 요소의 위치 초기화
        element.style.translate = new Translate(0, 0, 0);
    }

    private void OnChocolateDropped(VisualElement mold, string chocolateType)
    {
        
    }

    // UI 활성화
    private void OpenToggleUI() {  
        isUIActive = true;
        shapeUI.style.display = DisplayStyle.Flex;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerScript.SetControlState(false);
    }

    // UI 비활성화
    private void CloseToggleUI() {
        isUIActive = false;
        shapeUI.style.display = DisplayStyle.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerScript.SetControlState(true);
    }
}
