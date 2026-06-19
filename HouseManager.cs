using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HouseManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public StaminaManager staminaManager;

    [Header("House Display Settings")]
    public SpriteRenderer houseSpriteRenderer;
    public List<Sprite> houseStageSprites = new List<Sprite>();

    [Header("UI Ingredient Slots (3 Slots Total)")]
    public List<Image> ingredientIcons = new List<Image>();
    public List<TextMeshProUGUI> ingredientTexts = new List<TextMeshProUGUI>();

    [Header("Ingredient Sprite Assets")]
    public Sprite weedSprite;
    public Sprite twigSprite;
    public Sprite rockSprite;

    private int currentStage = 0;

    void Start()
    {
        if (houseSpriteRenderer != null && currentStage == 0)
        {
            houseSpriteRenderer.enabled = false;
        }
        UpdateRequiredIngredientsUI();
    }

    public bool TryUpgradeHouse()
    {
        if (currentStage >= 5)
        {
            Debug.Log("[House] Already reached Max Stage!");
            return false;
        }

        int nextStage = currentStage + 1;
        int reqWeed = GetRequiredCount(nextStage, "Seed");
        int reqTwig = GetRequiredCount(nextStage, "Branch");
        int reqRock = GetRequiredCount(nextStage, "Stone");

        bool hasWeed = reqWeed == 0 || inventoryManager.HasItemAmount("Seed", reqWeed);
        bool hasTwig = reqTwig == 0 || inventoryManager.HasItemAmount("Branch", reqTwig);
        bool hasRock = reqRock == 0 || inventoryManager.HasItemAmount("Stone", reqRock);

        if (hasWeed && hasTwig && hasRock)
        {
            if (reqWeed > 0) inventoryManager.RemoveItemAmount("Seed", reqWeed);
            if (reqTwig > 0) inventoryManager.RemoveItemAmount("Branch", reqTwig);
            if (reqRock > 0) inventoryManager.RemoveItemAmount("Stone", reqRock);

            // 🎯 [임무 연동 체크용] 업그레이드 전의 이전 단계를 백업
            int previousStage = currentStage;

            currentStage = nextStage;

            if (houseSpriteRenderer != null)
            {
                houseSpriteRenderer.enabled = true;
                int spriteIndex = currentStage - 1;
                if (spriteIndex < houseStageSprites.Count && houseStageSprites[spriteIndex] != null)
                {
                    houseSpriteRenderer.sprite = houseStageSprites[spriteIndex];
                }
            }

            if (staminaManager != null)
            {
                staminaManager.ModifyMaxStamina(10f);
            }

            // 🎯 [임무 연동] 건설 및 업그레이드 판단 로직
            if (QuestManager.Instance != null)
            {
                if (previousStage == 0)
                {
                    // 단계가 0에서 1이 되었다면 최초 건축 성공 -> 3번 임무 달성 (Index 2)
                    QuestManager.Instance.ProgressQuest(2, 1);
                }
                else if (previousStage >= 1)
                {
                    // 이미 지어진 집(1단계 이상)을 올렸다면 증축 성공 -> 7번 임무 달성 (Index 6)
                    QuestManager.Instance.ProgressQuest(6, 1);
                }
            }

            UpdateRequiredIngredientsUI();
            Debug.Log("[House] Successfully upgraded to Stage " + currentStage);
            return true;
        }
        else
        {
            Debug.LogWarning("[House] Missing required materials for upgrade!");
            return false;
        }
    }

    public void UpdateRequiredIngredientsUI()
    {
        int nextStage = currentStage + 1;

        if (nextStage > 5)
        {
            foreach (var icon in ingredientIcons) if (icon != null) icon.gameObject.SetActive(false);
            foreach (var txt in ingredientTexts) if (txt != null) txt.text = "";
            return;
        }

        int weed = GetRequiredCount(nextStage, "Seed");
        int twig = GetRequiredCount(nextStage, "Branch");
        int rock = GetRequiredCount(nextStage, "Stone");

        for (int i = 0; i < 3; i++)
        {
            if (ingredientIcons[i] != null) ingredientIcons[i].gameObject.SetActive(false);
            if (ingredientTexts[i] != null) ingredientTexts[i].text = "";
        }

        int currentSlotIndex = 0;

        if (weed > 0 && currentSlotIndex < 3)
        {
            ConfigureSlot(currentSlotIndex, weedSprite, weed);
            currentSlotIndex++;
        }
        if (twig > 0 && currentSlotIndex < 3)
        {
            ConfigureSlot(currentSlotIndex, twigSprite, twig);
            currentSlotIndex++;
        }
        if (rock > 0 && currentSlotIndex < 3)
        {
            ConfigureSlot(currentSlotIndex, rockSprite, rock);
            currentSlotIndex++;
        }
    }

    private void ConfigureSlot(int slotIndex, Sprite itemSprite, int count)
    {
        if (ingredientIcons[slotIndex] != null)
        {
            ingredientIcons[slotIndex].gameObject.SetActive(true);
            ingredientIcons[slotIndex].sprite = itemSprite;
        }
        if (ingredientTexts[slotIndex] != null)
        {
            ingredientTexts[slotIndex].text = count.ToString();
        }
    }

    private int GetRequiredCount(int stage, string itemName)
    {
        if (itemName == "Seed")
        {
            if (stage == 1 || stage == 2 || stage == 3) return 10;
            if (stage == 4) return 15;
            if (stage == 5) return 20;
        }
        else if (itemName == "Branch")
        {
            if (stage == 1) return 0;
            if (stage == 2 || stage == 3) return 10;
            if (stage == 4) return 15;
            if (stage == 5) return 20;
        }
        else if (itemName == "Stone")
        {
            if (stage == 1 || stage == 2) return 0;
            if (stage == 3) return 10;
            if (stage == 4) return 15;
            if (stage == 5) return 20;
        }
        return 0;
    }
}
