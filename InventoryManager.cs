using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public Image iconImage;
        public TextMeshProUGUI countText;
        public Button slotButton;
        public string storedItemName = "";
        [HideInInspector] public int itemCount = 0;
    }

    public List<InventorySlot> slots = new List<InventorySlot>();
    public EconomyManager economyManager;
    public StaminaManager staminaManager;

    void Start()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            if (slots[index].iconImage != null) slots[index].iconImage.enabled = false;
            if (slots[index].countText != null) slots[index].countText.text = "";

            if (slots[index].slotButton != null)
            {
                slots[index].slotButton.onClick.AddListener(() => OnClickSlot(index));
            }
        }
    }

    public void AddItem(string itemName, Sprite itemIcon)
    {
        foreach (var slot in slots)
        {
            if (slot.storedItemName == itemName)
            {
                slot.itemCount++;
                slot.countText.text = slot.itemCount.ToString();
                return;
            }
        }

        foreach (var slot in slots)
        {
            if (slot.itemCount == 0)
            {
                slot.storedItemName = itemName;
                slot.itemCount = 1;
                slot.iconImage.sprite = itemIcon;
                slot.iconImage.enabled = true;
                slot.countText.text = "1";
                return;
            }
        }
    }

    // [집 건설용 추가 1] 인벤토리에 해당 재료 수량이 충분한지 검사
    public bool HasItemAmount(string itemName, int amount)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.storedItemName == itemName)
            {
                total += slot.itemCount;
            }
        }
        return total >= amount;
    }

    // [집 건설용 추가 2] 인벤토리에서 지정한 개수만큼 재료 차감 처리
    public void RemoveItemAmount(string itemName, int amountToRemove)
    {
        foreach (var slot in slots)
        {
            if (slot.storedItemName == itemName && slot.itemCount > 0)
            {
                if (slot.itemCount >= amountToRemove)
                {
                    slot.itemCount -= amountToRemove;
                    amountToRemove = 0;
                }
                else
                {
                    amountToRemove -= slot.itemCount;
                    slot.itemCount = 0;
                }

                // UI 슬롯 동기화 정리
                if (slot.itemCount <= 0)
                {
                    slot.storedItemName = "";
                    slot.iconImage.sprite = null;
                    slot.iconImage.enabled = false;
                    slot.countText.text = "";
                }
                else
                {
                    slot.countText.text = slot.itemCount.ToString();
                }

                if (amountToRemove <= 0) break;
            }
        }
    }

    public void OnClickSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        InventorySlot slot = slots[index];

        if (slot.itemCount <= 0 || string.IsNullOrEmpty(slot.storedItemName)) return;

        if (slot.storedItemName == "Pudding")
        {
            if (staminaManager != null)
            {
                staminaManager.RestoreStamina(15f);
                RemoveItemAmount(slot.storedItemName, 1);
            }
        }
        else if (slot.storedItemName == "Juice")
        {
            if (staminaManager != null)
            {
                staminaManager.RestoreStamina(25f);
                RemoveItemAmount(slot.storedItemName, 1);
            }
        }
    }

    public void SellAllItems()
    {
        if (economyManager == null) return;

        int totalEarned = 0;
        foreach (var slot in slots)
        {
            if (slot.itemCount > 0)
            {
                int pricePerItem = economyManager.GetItemPrice(slot.storedItemName);
                if (pricePerItem > 0)
                {
                    totalEarned += pricePerItem * slot.itemCount;
                    slot.storedItemName = "";
                    slot.itemCount = 0;
                    slot.iconImage.sprite = null;
                    slot.iconImage.enabled = false;
                    slot.countText.text = "";
                }
            }
        }
        economyManager.AddMoney(totalEarned);
    }
}
