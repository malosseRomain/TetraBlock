using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverSound;
    private Button button;
    private AudioSource audioSource;

    public Slider soundEffectsSlider;

    private void Start()
    {
        button = GetComponent<Button>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }

        if (soundEffectsSlider != null)
        {
            float localVolume = soundEffectsSlider.value;
            audioSource.volume = localVolume / 100f;
        }
    }
}
