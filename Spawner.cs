using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnItem
    {
        public string itemName;
        public GameObject prefab;
        public int maxCount;
        [HideInInspector] public List<GameObject> spawnedObjects = new List<GameObject>();
    }

    public List<SpawnItem> itemList;

    [Header("Collider Settings")]
    public Collider2D seaCollider;

    [Header("Layer Settings")]
    public string targetLayerName = "Aea";

    [Header("Spawn Settings")]
    public float checkInterval = 0.5f;

    private int aeaLayerMask;
    private float timer = 0f;

    void Start()
    {
        if (seaCollider == null)
        {
            Debug.LogError("[Spawner] Sea Collider가 지정되지 않았습니다!");
            return;
        }
        aeaLayerMask = LayerMask.GetMask(targetLayerName);
        SpawnAllItems();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            SpawnAllItems();
        }
    }

    void SpawnAllItems()
    {
        bool itemsRemaining = true;
        int safetyNet = 0;
        int maxAttempts = 5000;
        int successCount = 0;

        Bounds bounds = seaCollider.bounds;

        while (itemsRemaining && safetyNet < maxAttempts)
        {
            safetyNet++;
            itemsRemaining = false;
            List<SpawnItem> activeList = new List<SpawnItem>();

            // 파괴된 오브젝트 정리 및 스폰이 더 필요한 아이템 수집
            foreach (var item in itemList)
            {
                item.spawnedObjects.RemoveAll(obj => obj == null);

                if (item.spawnedObjects.Count < item.maxCount)
                {
                    activeList.Add(item);
                    itemsRemaining = true;
                }
            }

            // 스폰할 아이템이 남아있다면 무작위로 하나 골라 스폰 시도
            if (activeList.Count > 0)
            {
                int randomIndex = Random.Range(0, activeList.Count);
                SpawnItem targetItem = activeList[randomIndex];

                // Collider 영역(Bounds) 내에서 무작위 좌표 생성
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomY = Random.Range(bounds.min.y, bounds.max.y);
                Vector2 spawnPos = new Vector2(randomX, randomY);

                // [핵심 변경] seaCollider 내부이면서(!), Aea 레이어 영역은 아닐 때만 생성
                if (seaCollider.OverlapPoint(spawnPos) && !Physics2D.OverlapPoint(spawnPos, aeaLayerMask))
                {
                    GameObject newObj = InstantiateObject(targetItem.prefab, spawnPos);
                    if (newObj != null)
                    {
                        targetItem.spawnedObjects.Add(newObj);
                        successCount++;
                    }
                }
            }
        }

        if (successCount > 0)
        {
            Debug.Log("[Spawner] 리스폰 완료된 개수: " + successCount);
        }
    }

    private GameObject InstantiateObject(GameObject prefab, Vector2 position)
    {
        if (prefab == null) return null;
        return Instantiate(prefab, position, Quaternion.identity);
    }
}
