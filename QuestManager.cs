using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [System.Serializable]
    public class QuestItem
    {
        public string questName;
        public int currentCount = 0;
        public int targetCount = 1;
        public bool isCompleted = false;
    }

    [Header("Quest List")]
    public QuestItem[] quests;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (quests != null && quests.Length > 1)
        {
            if (quests[0] != null) { quests[0].questName = "잡초 채집"; quests[0].targetCount = 10; }
            if (quests[1] != null) { quests[1].questName = "나뭇가지 채집"; quests[1].targetCount = 10; }
        }
        RefreshQuestUI();
    }

    public void ProgressQuest(int questIndex, int amount = 1)
    {
        if (quests == null || questIndex < 0 || questIndex >= quests.Length) return;

        QuestItem q = quests[questIndex];
        if (q.isCompleted) return;

        q.currentCount += amount;
        Debug.Log($"[카운트] {q.questName} : {q.currentCount} / {q.targetCount}");

        if (q.currentCount >= q.targetCount)
        {
            CompleteQuest(questIndex);
        }
    }

    public void CompleteQuest(int questIndex)
    {
        if (quests == null || questIndex < 0 || questIndex >= quests.Length) return;

        QuestItem q = quests[questIndex];
        q.isCompleted = true;
        q.currentCount = q.targetCount;

        RefreshQuestUI();
        Debug.Log($"★ 임무 최종 완료 : {q.questName}");
    }

    public void RefreshQuestUI()
    {
        QuestPageManager pageManager = FindAnyObjectByType<QuestPageManager>();
        if (pageManager != null)
        {
            pageManager.UpdatePageUI();
        }
    }
}
