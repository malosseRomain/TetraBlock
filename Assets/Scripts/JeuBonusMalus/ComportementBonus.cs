using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ComportementBonus : MonoBehaviour
{
    BoardBonus board;
    DifficultyManager difficulte;
    public AnimatedTile animationBomb;

    bool changeSpeedActivated = false;
    Coroutine changeSpeedCoroutine = null;

    bool scoreMultiplierActivated = false;
    Coroutine scoreMultiplierCoroutine = null;


    private void Awake()
    {
        board = GetComponent<BoardBonus>();
        difficulte = gameObject.GetComponent<DifficultyManager>();
    }

    public void BombExplosion(Vector3Int pos)
    {
        StartCoroutine(board.RemoveColumnWithAnimation(pos.x, pos.y, animationBomb));
    }

    public void ChangeSpeed(float duration, float multiplicateur, bool bSpeed) //gérer en une seule fonction les changements de vitesse permet d'eviter qu ils s'annulent si les 2 sont apliques
    {
        if (changeSpeedActivated)
        {
            StopCoroutine(changeSpeedCoroutine);
        }
        changeSpeedActivated = true;
        changeSpeedCoroutine = StartCoroutine(ChangeSpeedValue(multiplicateur, duration, true));
    }

    public void ScoreMulti(float duration, float multiplicateur)
    {
        if (scoreMultiplierActivated)
        {
            StopCoroutine(scoreMultiplierCoroutine);
        }
        scoreMultiplierActivated = true;
        scoreMultiplierCoroutine = StartCoroutine(AffectMultiplicationScore(multiplicateur, duration));
    }

    private IEnumerator ChangeSpeedValue(float StepFactor, float TimeEffect, bool bSpeed)
    {
        float elapsedTime = 0f;
        PieceControllerRetro pieceControl = (PieceControllerRetro)board.movControl; //fonctionne car par reference

        while (elapsedTime < TimeEffect)
        {
            float initialStepDelay = (difficulte ? difficulte.GetSpeed() : pieceControl.stepDelay);
            float newSpeed = 0;

            if (pieceControl.isHardDropping)
                newSpeed = 0.02f;
            else if (bSpeed)
                newSpeed = initialStepDelay / StepFactor;
            else
                newSpeed = initialStepDelay * StepFactor;

            pieceControl.stepDelay = newSpeed;
            elapsedTime += Time.deltaTime;
            yield return null; // Attendre une frame
        }
    }

    private IEnumerator AffectMultiplicationScore(float MultiplicatorFactor, float TimeEffect)
    {
        board.score.multiplicateur *= (int)MultiplicatorFactor;
        yield return new WaitForSeconds(TimeEffect);
        board.score.multiplicateur /= (int)MultiplicatorFactor;
    }
}
