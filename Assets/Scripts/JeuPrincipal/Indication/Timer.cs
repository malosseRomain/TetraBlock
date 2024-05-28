using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float startTime;

    private void Awake()
    {
        // Début du chronomètre
        startTime = Time.time;
    }

    private void Update()
    {
        // Temps écoulé depuis le début
        float elapsedTime = Time.time - startTime;

        // Convertit le temps écoulé en minutes et secondes
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Met à jour l'affichage du temps au format mm:ss
        GetComponent<Text>().text = "Chrono: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
