using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAndApplyVolume(); // 시작할 때 볼륨 불러오기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAndApplyVolume()
    {
        float savedBGM = PlayerPrefs.GetFloat("BGM_Volume_Key", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("SFX_Volume_Key", 1.0f);
        float savedPET = PlayerPrefs.GetFloat("PET_Volume_Key", 1.0f); // PET 추가

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);
        SetPETVolume(savedPET); // PET 추가
    }

    public void SetBGMVolume(float volume)
    {
        if (volume <= 0.0001f) audioMixer.SetFloat("BGM_Vol", -80f);
        else audioMixer.SetFloat("BGM_Vol", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("BGM_Volume_Key", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.0001f) audioMixer.SetFloat("SFX_Vol", -80f);
        else audioMixer.SetFloat("SFX_Vol", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("SFX_Volume_Key", volume);
        PlayerPrefs.Save();
    }

    // PET 볼륨 조절 함수 추가
    public void SetPETVolume(float volume)
    {
        if (volume <= 0.0001f) audioMixer.SetFloat("PET_Vol", -80f);
        else audioMixer.SetFloat("PET_Vol", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("PET_Volume_Key", volume);
        PlayerPrefs.Save();
    }
}
