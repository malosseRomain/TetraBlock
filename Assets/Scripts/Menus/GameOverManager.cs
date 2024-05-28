using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public bool gameOverActive = false;
    public GameObject gameOverObject;
    public Score scoreScript;
    public IBoard board;
    public SaveData saveDataScript;

    private void Start()
    {
        gameOverObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (!gameOverActive)
        {
            gameOverActive = true;
            gameOverObject.SetActive(true);
            saveDataScript.AddScore(scoreScript.maxScore);
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Retablissez le temps normal du jeu.
        SceneManager.LoadScene(0); // Chargez la scene du menu principal.
    }

    public void Replay()
    {
        Time.timeScale = 1f; // Retablissez le temps normal du jeu.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Rechargez la scene actuelle.
        scoreScript.RestartScore();
    }
}
