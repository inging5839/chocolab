using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreenController : MonoBehaviour
{
    private void OnEnable()
    {
        // UI Document 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 버튼 가져오기
        Button startButton = root.Q<Button>("StartButton");

        // 버튼 클릭 이벤트 연결
        startButton.clicked += OnStartButtonClick;
    }

    private void OnStartButtonClick()
    {
        // 다음 씬으로 이동
        SceneManager.LoadScene("GameScene");
    }
}
