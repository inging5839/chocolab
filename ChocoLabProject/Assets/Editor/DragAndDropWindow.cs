using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DragAndDropWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/Drag And Drop")]
    public static void ShowExample()
    {
        DragAndDropWindow wnd = GetWindow<DragAndDropWindow>();
        wnd.titleContent = new GUIContent("Drag And Drop");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

          // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DragAndDropWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DragAndDropWindow.uss");

        DragAndDropManipulator manipulator_1 =
        new(rootVisualElement.Q<VisualElement>("Dark"));
        DragAndDropManipulator manipulator_2 =
        new(rootVisualElement.Q<VisualElement>("Milk"));
        DragAndDropManipulator manipulator_3 =
        new(rootVisualElement.Q<VisualElement>("Strawberry"));
        DragAndDropManipulator manipulator_4 =
        new(rootVisualElement.Q<VisualElement>("Matcha"));

    }
}