using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour
{
    public Slider musicSlider;
    public AudioSource musicAudioSource;

    private void Start()
    {
        musicSlider.minValue = 0;
        musicSlider.maxValue = 100;
        musicSlider.value = 50;

        musicSlider.onValueChanged.AddListener(ChangeVolume);
    }

    private void ChangeVolume(float volume)
    {
        float normalizedVolume = volume / 100f;
        musicAudioSource.volume = normalizedVolume;
    }
}
