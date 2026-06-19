using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public InventoryManager inventory;
    public ShopManager shopManager;
    public StaminaManager staminaManager;

    [Header("UI Panels")]
    public GameObject closetPanel;
    public GameObject treasurePanel;
    public GameObject signPanel;
    public GameObject endPanel; // 🎯 엔드 패널 등록용 변수

    private ItemObject nearbyItem;
    private bool isNearShop = false;
    private bool isNearCloset = false;
    private bool isNearTreasure = false;
    private bool isNearSign = false;
    private bool isNearEnd = false; // 🎯 End 오브젝트 인접 여부

    private bool isClosetOpen = false;
    private bool isTreasureOpen = false;
    private bool isSignOpen = false;
    private bool isEndOpen = false; // 🎯 End 패널 오픈 상태 플래그

    private HouseManager nearbyHouseManager;
    private TreasureChest nearbyChest;

    void Start()
    {
        if (closetPanel != null) closetPanel.SetActive(false);
        if (treasurePanel != null) treasurePanel.SetActive(false);
        if (signPanel != null) signPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false); // 🎯 시작할 때 엔드 패널 비활성화
    }

    void Update()
    {
        // 1. Pick up item (잡초 10개 채집 / 나뭇가지 10개 채집 임무 연동)
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[상호작용] E키 입력 감지됨. 대상 아이템: {nearbyItem.itemName}");

            if (staminaManager != null)
            {
                if (staminaManager.HasEnoughStamina(1f))
                {
                    staminaManager.UseStamina(1f);
                    inventory.AddItem(nearbyItem.itemName, nearbyItem.itemIcon);

                    // 🎯 [임무 연동] 아이템 이름에 맞게 퀘스트 카운트 증가
                    if (QuestManager.Instance != null)
                    {
                        // 혹시 모를 앞뒤 공백을 제거하여 철자 검사를 안전하게 만듭니다.
                        string trimmedItemName = nearbyItem.itemName.Trim();
                        Debug.Log($"[퀘스트 체크] 공백 제거된 아이템 이름: '{trimmedItemName}'");

                        // 💡 [수정] 한국어 '잡초'뿐만 아니라 인스펙터에 적힌 영어 'Seed'나 'Weed'도 잡초 퀘스트로 인정합니다.
                        if (trimmedItemName == "잡초" || trimmedItemName == "Seed" || trimmedItemName == "Weed")
                        {
                            Debug.Log("[퀘스트 체크] '잡초/Seed/Weed' 일치 확인! QuestManager 0번 인덱스 호출.");
                            QuestManager.Instance.ProgressQuest(0, 1);
                        }
                        // 💡 [보완] 나뭇가지 역시 인스펙터에 'Branch'나 'Twig'로 적혀있을 경우를 대비해 예외 처리를 추가했습니다.
                        else if (trimmedItemName == "나뭇가지" || trimmedItemName == "Branch" || trimmedItemName == "Twig")
                        {
                            Debug.Log("[퀘스트 체크] '나뭇가지/Branch/Twig' 일치 확인! QuestManager 1번 인덱스 호출.");
                            QuestManager.Instance.ProgressQuest(1, 1);
                        }
                        else
                        {
                            Debug.LogWarning($"[퀘스트 체크] 아이템 이름 '{trimmedItemName}'이 일치하는 퀘스트 항목이 없습니다. ItemObject의 이름을 다시 확인하세요.");
                        }
                    }
                    else
                    {
                        Debug.LogError("[퀘스트 체크] 하이 hierarchy(씬)에 QuestManager 오브젝트가 없거나 복제되어 파괴되었습니다!");
                    }

                    Destroy(nearbyItem.gameObject);
                    nearbyItem = null;
                }
                else
                {
                    Debug.LogWarning("[상호작용 실패] 스태미나가 부족합니다.");
                }
            }
        }

        // 2. Open/Close Shop
        if (isNearShop && Input.GetKeyDown(KeyCode.E))
        {
            if (shopManager != null)
            {
                shopManager.ToggleShop();
            }
        }

        // 3. Open/Close Closet
        if (isNearCloset && Input.GetKeyDown(KeyCode.E))
        {
            if (closetPanel != null)
            {
                isClosetOpen = !isClosetOpen;
                closetPanel.SetActive(isClosetOpen);
            }
        }

        // 4. Open Treasure Chest (보물상자 4개 열기 임무 연동)
        if (isNearTreasure && Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyChest != null)
            {
                bool openedNow = nearbyChest.TryOpenChest(inventory);
                if (openedNow)
                {
                    // 🎯 [임무 연동] 5번 인덱스: 보물상자 개방 카운트 증가
                    if (QuestManager.Instance != null)
                    {
                        QuestManager.Instance.ProgressQuest(5, 1);
                    }
                }
            }
        }

        // 5. Open/Close Sign (집 업그레이드 하기 임무 연동)
        if (isNearSign && Input.GetKeyDown(KeyCode.E))
        {
            if (signPanel != null)
            {
                if (!isSignOpen)
                {
                    isSignOpen = true;
                    signPanel.SetActive(true);
                    if (nearbyHouseManager != null) nearbyHouseManager.UpdateRequiredIngredientsUI();
                }
                else
                {
                    if (nearbyHouseManager != null)
                    {
                        bool success = nearbyHouseManager.TryUpgradeHouse();
                        if (success)
                        {
                            isSignOpen = false;
                            signPanel.SetActive(false);

                            // 🎯 [임무 연동] 6번 인덱스: 집 업그레이드 성공 처리
                            if (QuestManager.Instance != null)
                            {
                                QuestManager.Instance.ProgressQuest(6, 1);
                            }
                        }
                    }
                }
            }
        }

        // 6. Open/Close End Panel 🎯 End 오브젝트 상호작용 (E키)
        if (isNearEnd && Input.GetKeyDown(KeyCode.E))
        {
            if (endPanel != null)
            {
                isEndOpen = !isEndOpen;
                endPanel.SetActive(isEndOpen);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            nearbyItem = collision.GetComponent<ItemObject>();
        }

        if (collision.CompareTag("Shop"))
        {
            isNearShop = true;
        }

        if (collision.CompareTag("Closet"))
        {
            isNearCloset = true;
        }

        if (collision.CompareTag("Treasure"))
        {
            isNearTreasure = true;
            nearbyChest = collision.GetComponent<TreasureChest>();
        }

        if (collision.CompareTag("Sign"))
        {
            isNearSign = true;
            nearbyHouseManager = collision.GetComponent<HouseManager>();
        }

        if (collision.CompareTag("End"))
        {
            isNearEnd = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && nearbyItem != null)
        {
            if (collision.gameObject == nearbyItem.gameObject) nearbyItem = null;
        }

        if (collision.CompareTag("Shop"))
        {
            isNearShop = false;
            if (shopManager != null) shopManager.CloseShop();
        }

        if (collision.CompareTag("Closet"))
        {
            isNearCloset = false;
            isClosetOpen = false;
            if (closetPanel != null) closetPanel.SetActive(false);
        }

        if (collision.CompareTag("Treasure"))
        {
            isNearTreasure = false;
            nearbyChest = null;
        }

        if (collision.CompareTag("Sign"))
        {
            isNearSign = false;
            isSignOpen = false;
            nearbyHouseManager = null;
            if (signPanel != null) signPanel.SetActive(false);
        }

        if (collision.CompareTag("End"))
        {
            isNearEnd = false;
            isEndOpen = false;
            if (endPanel != null) endPanel.SetActive(false);
        }
    }
}
