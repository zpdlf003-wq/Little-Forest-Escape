using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel; // 인스펙터에서 상점 UI 창 오브젝트를 드래그해서 넣어줍니다.
    private bool isShopOpen = false;

    void Start()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false); // 게임 시작할 때는 상점 창을 숨깁니다.
        }
    }

    public void ToggleShop()
    {
        if (shopPanel == null) return;

        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen); // 상태에 따라 켜거나 끕니다.

        // 상점 창이 열리면 게임을 일시정지하고 싶다면 아래 주석을 해제하세요.
        // Time.timeScale = isShopOpen ? 0f : 1f;
    }

    public void CloseShop()
    {
        if (shopPanel == null) return;
        isShopOpen = false;
        shopPanel.SetActive(false);
        // Time.timeScale = 1f;
    }
}
