using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderLink : MonoBehaviour
{
    // 1. PET 항목 추가
    public enum SoundType { BGM, SFX, PET }
    public SoundType soundType; // 인스펙터에서 BGM, SFX, PET 중 고르는 옵션

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // 설정창이 화면에 켜질 때마다 실행되는 유니티 내장 함수
    private void OnEnable()
    {
        if (slider == null) slider = GetComponent<Slider>();

        // 기존 연결 초기화
        slider.onValueChanged.RemoveAllListeners();

        // 사운드 타입에 따라 저장된 값을 불러와 슬라이더 위치 맞추기
        if (soundType == SoundType.BGM)
        {
            float savedBGM = PlayerPrefs.GetFloat("BGM_Volume_Key", 1.0f);
            slider.value = savedBGM;

            // 값이 바뀔 때 SoundManager 제어하기
            slider.onValueChanged.AddListener((val) => {
                if (SoundManager.instance != null) SoundManager.instance.SetBGMVolume(val);
            });
        }
        else if (soundType == SoundType.SFX)
        {
            float savedSFX = PlayerPrefs.GetFloat("SFX_Volume_Key", 1.0f);
            slider.value = savedSFX;

            slider.onValueChanged.AddListener((val) => {
                if (SoundManager.instance != null) SoundManager.instance.SetSFXVolume(val);
            });
        }
        // 2. PET 조건문 추가
        else if (soundType == SoundType.PET)
        {
            float savedPET = PlayerPrefs.GetFloat("PET_Volume_Key", 1.0f);
            slider.value = savedPET;

            slider.onValueChanged.AddListener((val) => {
                if (SoundManager.instance != null) SoundManager.instance.SetPETVolume(val);
            });
        }
    }
}
