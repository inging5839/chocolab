using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using Cursor = UnityEngine.Cursor;

public enum ChocolateShape
{
    Kisses,     // 키세스 모양
    Pepero,     // 빼빼로 모양
    Heart,      // 하트 모양
    Regular     // 일반 초콜릿 모양
}

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

    private void SetupDraggableChocolates()
    {
        var chocolateTypes = new[] { "Dark", "Milk", "Strawberry", "Matcha" };
        var chocolateArea = rootElement.Q<VisualElement>("MeltedChocolateArea");
        
        foreach (var type in chocolateTypes)
        {
            var chocolate = chocolateArea.Q<VisualElement>($"{type}Chocolate");
            if (chocolate != null)
            {
                MakeDraggable(chocolate);
            }
        }
    }

    private void SetupMolds()
    {
        var moldContainer = rootElement.Q<VisualElement>("ChocolateMoldArea");
        foreach (ChocolateShape shape in Enum.GetValues(typeof(ChocolateShape)))
        {
            var mold = new VisualElement();
            mold.name = $"Mold_{shape}";
            mold.AddToClassList("mold-slot");
            mold.AddToClassList($"shape-{shape.ToString().ToLower()}");
            
            var label = new Label(shape.ToString());
            label.AddToClassList("mold-label");
            mold.Add(label);
            
            moldContainer.Add(mold);
            MakeDroppable(mold);
        }
    }

    private void MakeDraggable(VisualElement element)
    {
        element.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (!isDragging)
            {
                draggedElement = element;
                startPosition = evt.mousePosition;
                isDragging = true;
                element.AddToClassList("dragging");
                evt.StopPropagation();
            }
        });

        element.RegisterCallback<MouseMoveEvent>(evt =>
        {
            if (isDragging && draggedElement == element)
            {
                Vector2 delta = evt.mousePosition - startPosition;
                element.style.translate = new Translate(delta.x, delta.y, 0);
                evt.StopPropagation();
            }
        });

        element.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (isDragging && draggedElement == element)
            {
                isDragging = false;
                draggedElement.RemoveFromClassList("dragging");
                CheckDropZone(evt.mousePosition);
                ResetElementPosition(element);
                draggedElement = null;
                evt.StopPropagation();
            }
        });
    }

    private void MakeDroppable(VisualElement mold)
    {
        mold.RegisterCallback<MouseEnterEvent>(evt =>
        {
            if (isDragging)
            {
                mold.AddToClassList("highlight");
            }
        });

        mold.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            mold.RemoveFromClassList("highlight");
        });
    }

    private void CheckDropZone(Vector2 position)
    {
        var molds = rootElement.Query<VisualElement>("ChocolateMold").ToList();
        foreach (var mold in molds)
        {
            if (IsPositionOverElement(position, mold))
            {
                OnChocolateDropped(mold, draggedElement.name);
                break;
            }
        }
    }

    private bool IsPositionOverElement(Vector2 position, VisualElement element)
    {
        var rect = element.worldBound;
        return rect.Contains(position);
    }

    private void ResetElementPosition(VisualElement element)
    {
        element.style.translate = new Translate(0, 0, 0);
    }

    private void OnChocolateDropped(VisualElement mold, string chocolateType)
    {
        string moldName = mold.name;
        ChocolateShape shape = (ChocolateShape)Enum.Parse(typeof(ChocolateShape), 
            moldName.Replace("Mold_", ""));

        mold.AddToClassList("filled-mold");
        mold.userData = new ChocolateInfo 
        { 
            Type = chocolateType,
            Shape = shape 
        };
    }

    private void OpenToggleUI() {  
        isUIActive = true;
        shapeUI.style.display = DisplayStyle.Flex;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerScript.SetControlState(false);
    }

    private void CloseToggleUI() {
        isUIActive = false;
        shapeUI.style.display = DisplayStyle.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerScript.SetControlState(true);
    }
}

public class ChocolateInfo
{
    public string Type { get; set; }
    public ChocolateShape Shape { get; set; }
}
