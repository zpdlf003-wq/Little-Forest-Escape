using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [System.Serializable]
    public class TreasureItem
    {
        public string itemName;
        public Sprite itemIcon;
    }

    [Header("Treasure List (Must be exactly 4 items)")]
    public TreasureItem goldBar;
    public TreasureItem moneyBag;
    public TreasureItem necklace;
    public TreasureItem paperMap;

    [Header("Visual Settings")]
    public Sprite openChestSprite; // Drag the OPENED chest sprite here

    private SpriteRenderer spriteRenderer;
    private bool isOpened = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // [Public] Called by PlayerInteract when pressing E near the chest
    public bool TryOpenChest(InventoryManager inventory)
    {
        // ❌ Prevent opening again if already opened
        if (isOpened) return false;
        if (inventory == null) return false;

        isOpened = true;

        // Change chest appearance to opened style
        if (spriteRenderer != null && openChestSprite != null)
        {
            spriteRenderer.sprite = openChestSprite;
        }

        // 🎲 Roll a 25% equal chance dice (0, 1, 2, 3)
        int roll = Random.Range(0, 4);
        TreasureItem rewardedItem = null;

        switch (roll)
        {
            case 0: rewardedItem = goldBar; break;
            case 1: rewardedItem = moneyBag; break;
            case 2: rewardedItem = necklace; break;
            case 3: rewardedItem = paperMap; break;
        }

        // Give the item to player inventory window
        if (rewardedItem != null)
        {
            inventory.AddItem(rewardedItem.itemName, rewardedItem.itemIcon);
            Debug.Log("[Chest] Reward given to inventory: " + rewardedItem.itemName);
        }

        return true;
    }
}
