using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // 유니티 에디터에서 튜토리얼 이미지(GameObject)를 드래그 앤 드롭으로 연결합니다.
    [SerializeField] private GameObject tutorialImage;
    [SerializeField] private float delayTime = 20f;

    void Start()
    {
        if (tutorialImage != null)
        {
            // 씬이 시작할 때 튜토리얼 이미지를 켭니다.
            tutorialImage.SetActive(true);

            // 20초 뒤에 이미지를 끄는 코루틴 시작
            StartCoroutine(HideTutorialAfterDelay());
        }
    }

    IEnumerator HideTutorialAfterDelay()
    {
        // 설정한 시간(20초) 동안 대기합니다.
        yield return new WaitForSeconds(delayTime);

        // 튜토리얼 이미지를 화면에서 숨깁니다.
        tutorialImage.SetActive(false);
    }
}
