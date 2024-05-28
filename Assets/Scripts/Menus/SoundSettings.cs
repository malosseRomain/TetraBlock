using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    public Slider globalVolumeSlider;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Audio Bouton", 50f);
        globalVolumeSlider.value = savedVolume;
    }

    public void OnGlobalVolumeChanged(float volume)
    {
        // Sauvegarde des player prefs
        PlayerPrefs.SetFloat("Audio Bouton", volume);
        PlayerPrefs.Save();
    }
}
