using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestPageManager : MonoBehaviour
{
    [Header("UI Slot References")]
    public Image leftDisplayImage;
    public Image rightDisplayImage;

    [Header("🎯 완료 표시 게임 오브젝트")]
    public GameObject leftCompleteStamp;
    public GameObject rightCompleteStamp;

    [Header("All Quest Sprites Data List")]
    public List<Sprite> allQuestSprites = new List<Sprite>();

    private int currentPageIndex = 0;

    void Start()
    {
        UpdatePageUI();
    }

    void OnEnable()
    {
        UpdatePageUI();
    }

    public void OnClickPrevPage()
    {
        if (allQuestSprites.Count < 2) return;
        currentPageIndex--;

        int maxPages = Mathf.CeilToInt(allQuestSprites.Count / 2f);
        if (currentPageIndex < 0) currentPageIndex = maxPages - 1;

        UpdatePageUI();
    }

    public void OnClickNextPage()
    {
        if (allQuestSprites.Count < 2) return;
        currentPageIndex++;

        int maxPages = Mathf.CeilToInt(allQuestSprites.Count / 2f);
        if (currentPageIndex >= maxPages) currentPageIndex = 0;

        UpdatePageUI();
    }

    public void UpdatePageUI()
    {
        if (leftDisplayImage == null || rightDisplayImage == null || allQuestSprites.Count == 0) return;

        int leftSpriteIndex = currentPageIndex * 2;
        int rightSpriteIndex = leftSpriteIndex + 1;

        // ==========================================
        // 1. 왼쪽 칸 처리
        // ==========================================
        if (leftSpriteIndex < allQuestSprites.Count && allQuestSprites[leftSpriteIndex] != null)
        {
            // 💡 현재 띄워진 퀘스트 데이터가 완료 상태일 때
            if (QuestManager.Instance != null && leftSpriteIndex < QuestManager.Instance.quests.Length && QuestManager.Instance.quests[leftSpriteIndex].isCompleted)
            {
                // 🎯 밑바탕의 검은 글자 원본 이미지를 완전히 숨겨서 글자가 가려지게 만듭니다.
                leftDisplayImage.gameObject.SetActive(false);

                // 완성 도장 마크 오브젝트만 화면에 켭니다.
                if (leftCompleteStamp != null) leftCompleteStamp.SetActive(true);
            }
            else
            {
                // 💡 완료되지 않은 상태라면 검은 글자 원본 이미지를 다시 띄웁니다.
                leftDisplayImage.sprite = allQuestSprites[leftSpriteIndex];
                leftDisplayImage.gameObject.SetActive(true);

                // 완성 도장은 숨깁니다.
                if (leftCompleteStamp != null) leftCompleteStamp.SetActive(false);
            }
        }
        else
        {
            leftDisplayImage.gameObject.SetActive(false);
            if (leftCompleteStamp != null) leftCompleteStamp.SetActive(false);
        }

        // ==========================================
        // 2. 오른쪽 칸 처리
        // ==========================================
        if (rightSpriteIndex < allQuestSprites.Count && allQuestSprites[rightSpriteIndex] != null)
        {
            // 💡 현재 띄워진 퀘스트 데이터가 완료 상태일 때
            if (QuestManager.Instance != null && rightSpriteIndex < QuestManager.Instance.quests.Length && QuestManager.Instance.quests[rightSpriteIndex].isCompleted)
            {
                // 🎯 밑바탕의 검은 글자 원본 이미지를 완전히 숨겨서 글자가 가려지게 만듭니다.
                rightDisplayImage.gameObject.SetActive(false);

                // 완성 도장 마크 오브젝트만 화면에 켭니다.
                if (rightCompleteStamp != null) rightCompleteStamp.SetActive(true);
            }
            else
            {
                // 💡 완료되지 않은 상태라면 검은 글자 원본 이미지를 다시 띄웁니다.
                rightDisplayImage.sprite = allQuestSprites[rightSpriteIndex];
                rightDisplayImage.gameObject.SetActive(true);

                // 완성 도장은 숨깁니다.
                if (rightCompleteStamp != null) rightCompleteStamp.SetActive(false);
            }
        }
        else
        {
            rightDisplayImage.gameObject.SetActive(false);
            if (rightCompleteStamp != null) rightCompleteStamp.SetActive(false);
        }
    }
}
