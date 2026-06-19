using UnityEngine;

// 1. 저장할 데이터 구조 정의
[System.Serializable]
public class SaveData
{
    public float playTime; // 플레이 타임 (초 단위)
    // 필요 시 여기에 정수형 점수(public int score;) 등을 추가할 수 있습니다.
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private float currentPlayTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 실시간으로 플레이 타임을 누적 계산합니다.
        currentPlayTime += Time.deltaTime;
    }

    // 현재 플레이 타임을 가져오는 함수
    public float GetCurrentPlayTime() => currentPlayTime;

    // 특정 슬롯(1, 2, 3)에 데이터를 저장하는 핵심 함수
    public void SaveGameSlot(int slotNumber)
    {
        SaveData newData = new SaveData();
        newData.playTime = currentPlayTime;

        // 구조체 데이터를 JSON 문자열로 변환하여 PlayerPrefs에 저장
        string json = JsonUtility.ToJson(newData);
        PlayerPrefs.SetString($"SaveSlot_{slotNumber}", json);
        PlayerPrefs.Save();

        Debug.Log($"{slotNumber}번 슬롯 저장 완료!");
    }

    // 특정 슬롯의 데이터를 불러오는 함수 (데이터가 없으면 null 반환)
    public SaveData LoadGameSlot(int slotNumber)
    {
        if (!PlayerPrefs.HasKey($"SaveSlot_{slotNumber}")) return null;

        string json = PlayerPrefs.GetString($"SaveSlot_{slotNumber}");
        return JsonUtility.FromJson<SaveData>(json);
    }
}
