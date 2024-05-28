using UnityEngine;

[System.Serializable]
public struct TresholdDifficulty
{
    public int ValueScore;
    public float Multiplicateur;
}

public class DifficultyManager : MonoBehaviour
{
    public TresholdDifficulty[] Paliers;
    private Score score;
    private int index = 0;
    private float initialStepDelay;
    private float currentSpeed;

    private void Awake()
    {
        BoardRetro board = GetComponent<BoardRetro>();
        PieceControllerRetro movControl = GetComponent<PieceControllerRetro>();

        score = board.score;
        initialStepDelay = movControl.stepDelay;
        currentSpeed = initialStepDelay;
    }

    public float GetSpeed()
    {
        if (score.maxScore > Paliers[index].ValueScore)
        {
            currentSpeed = initialStepDelay / Paliers[index].Multiplicateur;

            if (index < Paliers.Length - 1)
                index++;
        }

        return currentSpeed;
    }
}
