using UnityEngine;
using UnityEngine.UI;
using TMP_Text = TMPro.TMP_Text;

public class StaminaManager : MonoBehaviour
{
    public Slider staminaSlider;
    public TMP_Text staminaText;

    private float defaultMaxStamina = 25f;
    private float maxStamina = 25f;
    private float currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0f) currentStamina = 0f;
        UpdateStaminaUI();
    }

    public void RestoreStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    // 🎯 [핵심 수정] 기존 최대치에 인자로 들어온 수치만큼 '누적(+=' 추가)하여 영구 증가하도록 수정
    public void ModifyMaxStamina(float amount)
    {
        maxStamina += amount; // 25 -> 35 -> 45 순서대로 정상 누적됩니다.
        currentStamina += amount; // 보너스 수치만큼 현재 체력도 즉시 지급
        UpdateStaminaUI();
    }

    // 🎯 [펫 해제용 수정] 고양이 펫 해제 시에는 늘어난 양을 리셋하는 것이 아니라 기본 최대치 25로 강제 고정
    public void ResetMaxStamina()
    {
        maxStamina = defaultMaxStamina;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        if (staminaText != null)
        {
            staminaText.text = Mathf.RoundToInt(currentStamina) + " / " + Mathf.RoundToInt(maxStamina);
        }
    }
}
