using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClosetManager : MonoBehaviour
{
    public static ClosetManager Instance;

    [System.Serializable]
    public class ClothingItem
    {
        public string itemName;         // 옷 이름 ("탐험복", "원피스", "잠옷")
        public int price;                // 옷 가격 (100, 120, 150)
        public bool isPurchased = false; // 구매 완료 여부
        public bool isEquipped = false;  // 현재 장착 여부

        [Header("상점 UI 버튼")]
        public Button buyButton;

        [Header("옷장 UI 버튼")]
        public Button equipButton;

        [Header("이 옷의 16장 스프라이트 시트 배열")]
        public Sprite[] clothingSprites = new Sprite[16];
    }

    [Header("🎯 버튼 상태별 스프라이트 (이미지 3개 연결)")]
    public Sprite unpurchasedButtonSprite; // ⚪ 미구매 상태 이미지
    public Sprite equipButtonSprite;       // 🟢 장착 상태 이미지 
    public Sprite unequipButtonSprite;     // 🔴 해제 상태 이미지

    [Header("Player Reference")]
    public SpriteRenderer playerSpriteRenderer;

    [Header("Clothing List")]
    public List<ClothingItem> clothingList = new List<ClothingItem>();

    [HideInInspector] public bool hasCustomOutfit = false;
    [HideInInspector] public Sprite[] currentOutfitSprites;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ForceResetPurchasedData();
    }

    void Start()
    {
        InitItemSettings();
        UpdateAllUI();
    }

    private void ForceResetPurchasedData()
    {
        if (clothingList == null) return;

        foreach (var item in clothingList)
        {
            if (item != null)
            {
                item.isPurchased = false;
                item.isEquipped = false;
            }
        }
        hasCustomOutfit = false;
        currentOutfitSprites = null;
    }

    private void InitItemSettings()
    {
        foreach (var item in clothingList)
        {
            if (item.itemName == "탐험복") item.price = 100;
            else if (item.itemName == "원피스") item.price = 120;
            else if (item.itemName == "잠옷") item.price = 150;
        }
    }

    public void BuyClothing(int index)
    {
        if (index < 0 || index >= clothingList.Count) return;
        ClothingItem item = clothingList[index];

        if (item.isPurchased) return;

        if (EconomyManager.Instance != null && EconomyManager.Instance.HasEnoughMoney(item.price))
        {
            EconomyManager.Instance.DeductMoney(item.price);
            item.isPurchased = true;

            Debug.Log($"{item.itemName} 구매 성공!");
            UpdateAllUI();
        }
        else
        {
            Debug.LogWarning("돈이 부족하여 옷을 구매할 수 없습니다.");
        }
    }

    public void ToggleEquipClothing(int index)
    {
        if (index < 0 || index >= clothingList.Count) return;
        ClothingItem item = clothingList[index];

        if (!item.isPurchased) return;

        if (item.isEquipped)
        {
            item.isEquipped = false;
            hasCustomOutfit = false;
            currentOutfitSprites = null;
            Debug.Log($"{item.itemName} 해제 완료.");
        }
        else
        {
            foreach (var otherItem in clothingList)
            {
                otherItem.isEquipped = false;
            }

            item.isEquipped = true;
            hasCustomOutfit = true;
            currentOutfitSprites = item.clothingSprites;
            Debug.Log($"{item.itemName} 장착 완료.");
        }

        UpdateAllUI();
    }

    // 🎯 이미지 3개를 조건에 따라 완벽하게 덮어씌우는 UI 리프레시 함수
    public void UpdateAllUI()
    {
        for (int i = 0; i < clothingList.Count; i++)
        {
            ClothingItem item = clothingList[i];

            // 1. 상점 구매 버튼 상태 조절
            if (item.buyButton != null)
            {
                item.buyButton.interactable = !item.isPurchased;
            }

            // 2. 옷장 버튼 이미지 및 활성화 상태 제어
            if (item.equipButton != null)
            {
                // [A] 아직 사지 않은 옷장 버튼 처리
                if (!item.isPurchased)
                {
                    item.equipButton.interactable = false; // 클릭 불가 상태

                    // 💡 미구매 상태일 때 강제로 "미구매" 스크린샷 이미지를 박아넣습니다.
                    if (unpurchasedButtonSprite != null)
                    {
                        item.equipButton.image.sprite = unpurchasedButtonSprite;
                    }
                }
                // [B] 구매 완료한 옷장 버튼 처리
                else
                {
                    item.equipButton.interactable = true; // 클릭 가능 상태

                    // 장착 여부에 따라 장착(녹색) / 해제(빨간색) 이미지를 입힙니다.
                    if (item.isEquipped)
                    {
                        if (unequipButtonSprite != null) item.equipButton.image.sprite = unequipButtonSprite;
                    }
                    else
                    {
                        if (equipButtonSprite != null) item.equipButton.image.sprite = equipButtonSprite;
                    }
                }
            }
        }
    }
}
