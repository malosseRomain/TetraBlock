using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private int currentIndex = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextClip();
    }

    private void PlayNextClip()
    {
        if (audioClips.Length == 0)
            return;

        audioSource.clip = audioClips[currentIndex];
        audioSource.Play();

        currentIndex = (currentIndex + 1) % audioClips.Length;

        Invoke("PlayNextClip", audioSource.clip.length);
    }
}
