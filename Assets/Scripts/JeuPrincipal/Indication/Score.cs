using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    public IBoard board;
    private Text scoreText;
    [HideInInspector]
    public int maxScore;
    private int currentScore = 0;
    private int scoreDigits = 5;

    private Coroutine scoreCoroutine;

    public int multiplicateur { get; set; } = 1;

    void Start()
    {
        scoreText = GetComponentInChildren<Text>();
        RestartScore();
    }

    public void AddScore(int newScore)
    {
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }
        //Debug.Log("score ajouté");
        scoreCoroutine = StartCoroutine(IncrementScore(newScore * multiplicateur));
    }

    public void RestartScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    private IEnumerator IncrementScore(int newScore)
    {
        int incrementSpeed = 1;
        if (maxScore - currentScore > 50)
        {
            incrementSpeed = 5; 
        }

        while (currentScore < maxScore)
        {
            currentScore += incrementSpeed; 
            if (currentScore > maxScore) 
            {
                currentScore = maxScore;
            }
            UpdateScoreDisplay();
            yield return new WaitForSeconds(0.05f);
        }
    }


    private void UpdateScoreDisplay()
    {
        if (board.gameOverManager.gameOverActive)
            currentScore = maxScore;

        scoreText.text = "Score: " + currentScore.ToString("D" + scoreDigits);
        //Debug.Log(currentScore);
    }
}