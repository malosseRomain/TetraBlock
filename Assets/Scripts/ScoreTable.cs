using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO; // Importez cette directive pour gérer les fichiers.
using System.Linq;

public class ScoreTable : MonoBehaviour
{
    public Text scoreListText; // Le champ de texte où vous afficherez les scores.

    private List<int> scoresList = new List<int>();

    private string scoresFilePath = "Assets/highscores.txt"; // Chemin du fichier de scores.

    private void Start()
    {
        // Désactivez le champ de texte au démarrage.
        scoreListText.gameObject.SetActive(false);

        // Chargez les scores enregistrés depuis le fichier.
        LoadScores();
    }

    // Ajoutez un score au tableau des scores.
    public void AddScore(int newScore)
    {
        scoresList.Add(newScore);

        // Triez la liste des scores élevés du plus élevé au plus bas.
        scoresList = scoresList.OrderByDescending(score => score).ToList();

        // Assurez-vous que la liste ne dépasse pas une certaine taille (par exemple, 10 scores maximum).
        if (scoresList.Count > 10)
        {
            scoresList.RemoveAt(scoresList.Count - 1);
        }

        // Mettez à jour le texte du tableau des scores.
        UpdateScoreListText();

        // Activez le champ de texte pour afficher les scores.
        scoreListText.gameObject.SetActive(true);

        // Enregistrez les scores mis à jour dans le fichier.
        SaveScores();
    }

    private void UpdateScoreListText()
    {
        string scoreText = "High Scores:\n\n";

        while (scoresList.Count < 10)
        {
            scoresList.Add(0);
        }

        for (int i = 0; i < 10; i++) // Always display top 10 scores
        {
            scoreText += (i + 1) + ". " + scoresList[i] + "\n";
        }

        scoreListText.text = scoreText;
    }


    public void HideScoreTable()
    {
        // Désactivez le champ de texte pour masquer les scores.
        scoreListText.gameObject.SetActive(false);
    }

    // Enregistrez les scores dans un fichier texte.
    private void SaveScores()
    {
        string scoresString = string.Join("\n", scoresList.Select(score => score.ToString()).ToArray());
        File.WriteAllText(scoresFilePath, scoresString);
    }

    // Chargez les scores depuis un fichier texte.
    private void LoadScores()
    {
        if (!File.Exists(scoresFilePath))
            return;

        string scoresString = File.ReadAllText(scoresFilePath);
        string[] scoreLines = scoresString.Split('\n');

        scoresList.Clear();

        foreach (string scoreLine in scoreLines)
        {
            if (int.TryParse(scoreLine, out int score))
            {
                scoresList.Add(score);
            }
        }
    }
}
