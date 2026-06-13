using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private Slider MasterVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider UIVolumeSlider;
    [SerializeField] private string MasterSaveKey = "MasterVolume";
    [SerializeField] private string MusicSaveKey = "MusicVolume";
    [SerializeField] private string SFXSaveKey = "SFXVolume";
    [SerializeField] private string UISaveKey = "UIVolume";

    private void Start()
    {
        LoadAllVolume();
    }

    public void SetMasterVolume()
    {
        float volume = MasterVolumeSlider.value;
        Mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MasterSaveKey, volume);
    }
    public void SetMusicVolume()
    {
        float volume = MusicVolumeSlider.value;
        Mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MusicSaveKey, volume);
    }
    public void SetSFXVolume()
    {
        float volume = SFXVolumeSlider.value;
        Mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFXSaveKey, volume);
    }
    public void SetUIVolume()
    {
        float volume = UIVolumeSlider.value;
        Mixer.SetFloat("UI", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(UISaveKey, volume);
    }

    private void LoadAllVolume()
    {
        MasterVolumeSlider.value = PlayerPrefs.GetFloat(MasterSaveKey);
        MusicVolumeSlider.value = PlayerPrefs.GetFloat(MusicSaveKey);
        SFXVolumeSlider.value = PlayerPrefs.GetFloat(SFXSaveKey);
        UIVolumeSlider.value = PlayerPrefs.GetFloat(UISaveKey);


        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
        SetUIVolume();
    }
}
