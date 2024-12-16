using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using Random = System.Random;

public class Order : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootElement;
    private VisualElement orderUI;
    
    List<string[]> orderList = new List<string[]>();
    List<string> FlavorList = new List<string> {"Dark", "Milk", "Strawberry", "Matcha"};
    List<string> ShapeList = new List<string> {"Kisses", "Pepero", "Heart", "Star"};
    Random random = new Random();
    

    void Awake()
        {
            // UIDocument 컴포넌트 가져오기
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }
        }


    // Start is called before the first frame update
    void Start()
    {
        rootElement = uiDocument.rootVisualElement;
        orderUI = rootElement.Q<VisualElement>("OrderUI");
        orderUI.visible = false;
        var orderDisplay0 = orderUI.Q<VisualElement>("OrderDisplay0");
        var orderDisplay1 = orderUI.Q<VisualElement>("OrderDisplay1");
        var orderDisplay2 = orderUI.Q<VisualElement>("OrderDisplay2");
        var orderDisplay3 = orderUI.Q<VisualElement>("OrderDisplay3");
        VisualElement[] orderDisplays = {orderDisplay0, orderDisplay1, orderDisplay2, orderDisplay3};
        
        
        // 랜덤 주문 4개 생성
        for (int i = 0; i < 4; i++) {
            string[] newOrder = new string[2];
            var flavorText = orderDisplays[i].Q<Label>("Flavor");
            var shapeText = orderDisplays[i].Q<Label>("Shape");
            flavorText.text = FlavorList[random.Next(FlavorList.Count)];
            shapeText.text = ShapeList[random.Next(ShapeList.Count)];
            newOrder[0] = flavorText.text;
            newOrder[1] = shapeText.text;
            Debug.Log(newOrder[0] + " " + newOrder[1]);
            orderList.Add(newOrder);

        }
        
        orderUI.visible = true;





    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
