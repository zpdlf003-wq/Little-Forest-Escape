using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 필요합니다.

public class ToEnd : MonoBehaviour
{
    // 버튼에 연결할 함수 (인스펙터에서 씬 이름을 적을 수 있게 만듦)
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene("end");
    }
}
