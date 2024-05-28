using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float startTime;

    private void Awake()
    {
        // D�but du chronom�tre
        startTime = Time.time;
    }

    private void Update()
    {
        // Temps �coul� depuis le d�but
        float elapsedTime = Time.time - startTime;

        // Convertit le temps �coul� en minutes et secondes
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Met � jour l'affichage du temps au format mm:ss
        GetComponent<Text>().text = "Chrono: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
