using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameSceneController : MonoBehaviour
{
    private void OnEnable()
    {
        // UI Document 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 버튼 가져오기
        Button backButton = root.Q<Button>("BackButton");

        // 버튼 클릭 이벤트 연결
        backButton.clicked += OnBackButtonClick;
    }

    private void OnBackButtonClick()
    {
        // StartScene으로 이동
        SceneManager.LoadScene("StartScene");
    }
}
