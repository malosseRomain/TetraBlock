using UnityEngine;

public class AudioPieceMovements : MonoBehaviour
{
    public AudioClip moveLeftSound;
    public AudioClip moveRightSound;
    public AudioClip moveDownSound;
    public AudioClip moveUpSound;
    public AudioClip rotateSound;
    public AudioClip lockSound;
    public AudioClip RadarDownSound;
    public AudioClip RadarUpSound;
    public AudioClip RadarLeftSound;
    public AudioClip RadarRightSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMoveLeftSound()
    {
        PlaySound(moveLeftSound);
    }

    public void PlayMoveRightSound()
    {
        PlaySound(moveRightSound);
    }

    public void PlayMoveDownSound()
    {
        PlaySound(moveDownSound);
    }    
    
    public void PlayMoveUpSound()
    {
        PlaySound(moveUpSound);
    }

    public void PlayRotateSound()
    {
        PlaySound(rotateSound);
    }    
    
    public void PlayLockSound()
    {
        PlaySound(lockSound);
    }

    public void PlayRadarUpSound()
    {
       PlaySound(RadarUpSound);
    }

    public void PlayRadarDownSound()
    {
        PlaySound(RadarDownSound);
    }
    public void PlayRadarLeftSound()
    {
        PlaySound(RadarLeftSound);
    }
    public void PlayRadarRightSound()
    {
        PlaySound(RadarRightSound);
    }

    //private void PlaySound(AudioClip clip)
    //{
    //    if (clip != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Vérifiez si le clip est déjà en train de jouer
            if (!audioSource.isPlaying || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
