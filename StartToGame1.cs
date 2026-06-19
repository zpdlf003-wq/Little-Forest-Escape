using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 추가해야 합니다.

public class StartToGame1 : MonoBehaviour
{
    // 버튼 클릭 시 호출할 함수입니다.
    public void ChangeGameScene()
    {
        // "GameScene" 자리에 이동하고 싶은 실제 씬 이름을 넣으세요.
        SceneManager.LoadScene("game1");
    }
}
