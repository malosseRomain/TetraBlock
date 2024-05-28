using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsVolumeControl : MonoBehaviour
{
    public Slider soundEffectsSlider;

    private void Start()
    {
        soundEffectsSlider.minValue = 0;
        soundEffectsSlider.maxValue = 100;
        soundEffectsSlider.value = 50;
    }

    private void Update()
    {
        GameObject[] soundEffectObjects = GameObject.FindGameObjectsWithTag("SoundEffect");

        foreach (GameObject soundEffectObject in soundEffectObjects)
        {
            AudioSource soundEffectAudioSource = soundEffectObject.GetComponent<AudioSource>();
            if (soundEffectAudioSource != null)
            {
                float normalizedVolume = soundEffectsSlider.value / 100f;
                soundEffectAudioSource.volume = normalizedVolume;
            }
        }
    }
}
