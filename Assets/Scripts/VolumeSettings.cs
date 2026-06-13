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

    [SerializeField] AudioManager AudioManager;

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

    public void SetSFXVolumeByPlayer()
    {
        SetSFXVolume();
        if (AudioManager != null)
            AudioManager.PlaySFX(0);
    }

    public void SetSFXVolume()
    {
        float volume = SFXVolumeSlider.value;
        Mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFXSaveKey, volume);
    }

    public void SetUiVolumeByPlayer()
    {
        SetUIVolume();
        if (AudioManager != null)
            AudioManager.PlayUI(0);
    }

    public void SetUIVolume()
    {
        float volume = UIVolumeSlider.value;
        Mixer.SetFloat("UI", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(UISaveKey, volume);
    }


    private void LoadAllVolume()
    {
        MasterVolumeSlider.value = PlayerPrefs.GetFloat(MasterSaveKey,0.75f);
        MusicVolumeSlider.value = PlayerPrefs.GetFloat(MusicSaveKey, 0.75f);
        SFXVolumeSlider.value = PlayerPrefs.GetFloat(SFXSaveKey, 0.75f);
        UIVolumeSlider.value = PlayerPrefs.GetFloat(UISaveKey, 0.75f);

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
        SetUIVolume();

    }

    private void DisableSliderEvents(bool disable)
    {
        if (disable)
        {
            MasterVolumeSlider.onValueChanged.RemoveListener(_ => SetMasterVolume());
            MusicVolumeSlider.onValueChanged.RemoveListener(_ => SetMusicVolume());
            SFXVolumeSlider.onValueChanged.RemoveListener(_ => SetSFXVolume());
            UIVolumeSlider.onValueChanged.RemoveListener(_ => SetUIVolume());
        }
        else
        {
            MasterVolumeSlider.onValueChanged.AddListener(_ => SetMasterVolume());
            MusicVolumeSlider.onValueChanged.AddListener(_ => SetMusicVolume());
            SFXVolumeSlider.onValueChanged.AddListener(_ => SetSFXVolume());
            UIVolumeSlider.onValueChanged.AddListener(_ => SetUIVolume());
        }
    }

}
