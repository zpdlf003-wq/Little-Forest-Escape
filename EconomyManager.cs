using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EconomyManager : MonoBehaviour
{
    // 🎯 ClosetManager 등 외부 스크립트에서 접근할 수 있도록 싱글톤 인스턴스를 추가합니다.
    public static EconomyManager Instance;

    [Header("Item Prices")]
    public int weedPrice = 10;
    public int twigPrice = 20;
    public int rockPrice = 30;
    public int goldBarPrice = 500;
    public int necklacePrice = 1000;
    // [추가] 새 아이템 가격 변수
    public int moneyBagPrice = 2000;
    public int mapPiecePrice = 1500;

    [Header("UI References")]
    public List<TMP_Text> moneyTextList = new List<TMP_Text>();

    private int totalMoney = 0;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    public int GetItemPrice(string itemName)
    {
        switch (itemName)
        {
            case "Seed": return weedPrice;
            case "Branch": return twigPrice;
            case "Stone": return rockPrice;
            case "GoldBar": return goldBarPrice;
            case "Necklace": return necklacePrice;
            // [추가] 새 아이템 이름 매핑
            case "MoneyBag": return moneyBagPrice;
            case "MapPiece": return mapPiecePrice;
            default: return 0;
        }
    }

    // 돈이 충분히 있는지 검사하는 함수
    public bool HasEnoughMoney(int amount)
    {
        return totalMoney >= amount;
    }

    // 돈을 차감하는 함수
    public void DeductMoney(int amount)
    {
        totalMoney -= amount;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        totalMoney += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        foreach (TMP_Text txt in moneyTextList)
        {
            if (txt != null)
            {
                txt.text = totalMoney.ToString();
            }
        }
    }
}
