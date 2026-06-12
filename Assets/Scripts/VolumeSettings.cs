using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string mixerParameterName = "MasterVolume";
    [SerializeField] private string saveKey = "MasterVolume";

    private void Start()
    {
        // Загружаем сохраненную громкость
        LoadVolume();

        // Подписываемся на событие изменения значения слайдера
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void ShowCurrVolume()
    {

    }

    private void OnVolumeChanged(float volume)
    {
        // Применяем громкость
        SetVolume(volume);

        // Сохраняем значение
        SaveVolume(volume);
    }

    private void SetVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Конвертируем линейное значение (0-1) в децибелы (-80dB to 0dB)
            float volumeInDb = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
            audioMixer.SetFloat(mixerParameterName, volumeInDb);
        }
    }

    private void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat(saveKey, volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        if (volumeSlider != null)
        {
            // Загружаем сохраненную громкость или используем значение по умолчанию (0.75)
            float savedVolume = PlayerPrefs.GetFloat(saveKey, 0.75f);
            volumeSlider.value = savedVolume;

            // Применяем загруженную громкость
            SetVolume(savedVolume);
        }
    }

}
