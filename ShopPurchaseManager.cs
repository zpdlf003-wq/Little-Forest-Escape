using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopPurchaseManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItemData
    {
        public string itemID;
        public int price = 100;

        [Header("Type Settings")]
        public bool isPet = false;

        [Header("Shop UI References")]
        public Button buyButton;
        public Sprite soldOutSprite;

        [Header("Closet UI References")]
        public Button closetButton;
        public TextMeshProUGUI closetText;

        [Header("Closet Button Sprites")]
        public Sprite lockedSprite;
        public Sprite equipSprite;
        public Sprite unequipSprite;

        [Header("Asset References")]
        public GameObject petPrefab;
        public Sprite itemIconSprite;

        [HideInInspector] public bool isPurchased = false;
        [HideInInspector] public GameObject spawnedPetInstance = null;
    }

    [Header("Manager References")]
    public EconomyManager economyManager;
    public InventoryManager inventoryManager;
    public StaminaManager staminaManager;

    [Header("Player Target Settings")]
    public Transform playerTransform;
    public MonoBehaviour playerMovementScript;
    public string speedVariableName = "moveSpeed";

    [Header("Sea Bound Settings")]
    public Collider2D seaInvisibleWall;

    [Header("Shop Items List")]
    public List<ShopItemData> shopItems = new List<ShopItemData>();

    private ShopItemData currentlyActivePet = null;
    private float goldTimer = 0f;
    private float defaultSpeed = 5f;

    void Start()
    {
        foreach (var item in shopItems)
        {
            if (item.isPet)
            {
                if (item.closetButton != null && item.lockedSprite != null) item.closetButton.image.sprite = item.lockedSprite;
                if (item.closetText != null) item.closetText.text = "Locked";
            }
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        SaveDefaultPlayerSpeed();
    }

    void Update()
    {
        if (currentlyActivePet != null && currentlyActivePet.itemID == "BootPet")
        {
            goldTimer += Time.deltaTime;
            if (goldTimer >= 60f)
            {
                goldTimer = 0f;
                if (economyManager != null) economyManager.AddMoney(10);
            }
        }
    }

    public void BuyItemIndex(int index)
    {
        if (index < 0 || index >= shopItems.Count) return;
        ShopItemData item = shopItems[index];

        if (item.isPet && item.isPurchased) return;

        if (economyManager.HasEnoughMoney(item.price))
        {
            economyManager.DeductMoney(item.price);

            if (item.isPet)
            {
                item.isPurchased = true;
                item.buyButton.interactable = false;
                if (item.soldOutSprite != null) item.buyButton.image.sprite = item.soldOutSprite;

                if (item.closetButton != null && item.equipSprite != null) item.closetButton.image.sprite = item.equipSprite;
                if (item.closetText != null) item.closetText.text = "Equip";

                // 🎯 [임무 연동] 9번 임무: 꽁꽁 캣 입양하기 (Index 8)
                // 아이템 ID가 "IcePet"일 때 퀘스트 매니저 호출
                if (item.itemID == "IcePet" && QuestManager.Instance != null)
                {
                    QuestManager.Instance.ProgressQuest(8, 1);
                }
            }
            else
            {
                if (inventoryManager != null)
                {
                    inventoryManager.AddItem(item.itemID, item.itemIconSprite);
                }

                // 🎯 [임무 연동] 8번 임무: 옷 1개 구매하기 (Index 7)
                // 펫이 아닌 일반 아이템(옷) 구매 성공 시 퀘스트 매니저 호출
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.ProgressQuest(7, 1);
                }
            }
        }
    }

    public void ToggleClosetEquipIndex(int index)
    {
        if (index < 0 || index >= shopItems.Count) return;
        ShopItemData item = shopItems[index];

        if (!item.isPurchased || !item.isPet) return;

        if (currentlyActivePet == item)
        {
            RemovePetAbilities(item.itemID);
            if (item.spawnedPetInstance != null)
            {
                Destroy(item.spawnedPetInstance);
                item.spawnedPetInstance = null;
            }
            currentlyActivePet = null;

            if (item.closetButton != null && item.equipSprite != null) item.closetButton.image.sprite = item.equipSprite;
            if (item.closetText != null) item.closetText.text = "Equip";
        }
        else
        {
            if (currentlyActivePet != null)
            {
                RemovePetAbilities(currentlyActivePet.itemID);
                if (currentlyActivePet.spawnedPetInstance != null)
                {
                    Destroy(currentlyActivePet.spawnedPetInstance);
                    currentlyActivePet.spawnedPetInstance = null;
                }
                if (currentlyActivePet.closetButton != null && currentlyActivePet.equipSprite != null)
                    currentlyActivePet.closetButton.image.sprite = currentlyActivePet.equipSprite;
                if (currentlyActivePet.closetText != null) currentlyActivePet.closetText.text = "Equip";
            }

            if (item.petPrefab != null && playerTransform != null)
            {
                Vector3 spawnPos = playerTransform.position + new Vector3(-1f, 0f, 0f);
                item.spawnedPetInstance = Instantiate(item.petPrefab, spawnPos, Quaternion.identity);

                PetController petScript = item.spawnedPetInstance.GetComponent<PetController>();
                if (petScript != null) petScript.playerTransform = playerTransform;
            }

            currentlyActivePet = item;
            ApplyPetAbilities(item.itemID);

            if (item.closetButton != null && item.unequipSprite != null) item.closetButton.image.sprite = item.unequipSprite;
            if (item.closetText != null) item.closetText.text = "Unequip";
        }
    }

    private void ApplyPetAbilities(string petID)
    {
        switch (petID)
        {
            case "DogPet":
                ModifyPlayerSpeed(defaultSpeed + 1f);
                break;
            case "CatPet":
                if (staminaManager != null) staminaManager.ModifyMaxStamina(20f);
                break;
            case "SonyPet":
                ModifyPlayerSpeed(defaultSpeed + 3f);
                break;
            case "BootPet":
                goldTimer = 0f;
                break;
            case "IcePet":
                if (seaInvisibleWall != null) seaInvisibleWall.isTrigger = true;
                break;
        }
    }

    private void RemovePetAbilities(string petID)
    {
        switch (petID)
        {
            case "DogPet":
            case "SonyPet":
                ModifyPlayerSpeed(defaultSpeed);
                break;
            case "CatPet":
                if (staminaManager != null) staminaManager.ResetMaxStamina();
                break;
            case "IcePet":
                if (seaInvisibleWall != null) seaInvisibleWall.isTrigger = false;
                break;
        }
    }

    private void SaveDefaultPlayerSpeed()
    {
        if (playerMovementScript == null) return;
        System.Reflection.FieldInfo field = playerMovementScript.GetType().GetField(speedVariableName);
        if (field != null) defaultSpeed = (float)field.GetValue(playerMovementScript);
    }

    private void ModifyPlayerSpeed(float newSpeed)
    {
        if (playerMovementScript == null) return;
        System.Reflection.FieldInfo field = playerMovementScript.GetType().GetField(speedVariableName);
        if (field != null) field.SetValue(playerMovementScript, newSpeed);
    }
}
