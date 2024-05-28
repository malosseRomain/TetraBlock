using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool pauseActive { get; private set; } = false;
    public GameObject pausePanel;
    public GameOverManager gameOverManager;
    public Button buttonOption;
    public Button buttonRetour;
    public GameObject Fade;
    public Animator FadeSystem;

    private bool buttonOptionClicked = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        buttonOption.onClick.AddListener(OptionButtonClicked);
        buttonRetour.onClick.AddListener(RetourButtonClicked);
    }

    private void Update()
    {
        if (gameOverManager == null || gameOverManager.gameOverActive || buttonOptionClicked)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        pauseActive = !pauseActive;
        pausePanel.SetActive(pauseActive);
        StartCoroutine(StopShaking());
    }

    private IEnumerator StopShaking()
    {
        if (pauseActive)
        {
            yield return new WaitForSeconds(0.1f);
            StopAllCoroutines();
        }
        Time.timeScale = pauseActive ? 0f : 1f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(0);
    }

    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void OptionButtonClicked()
    {
        buttonOptionClicked = true;
    }

    private void RetourButtonClicked()
    {
        buttonOptionClicked = false;
    }
}
