using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardClassique : IBoard
{
    public TriominoData[] triominoes;
    public int distanceSpawnFrom = 4;
    private AudioSource audioSource ;
    public AudioClip lineSuppresion;

    ///////////////////////////////////// Event function

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();

        movControl = GetComponent<PieceControllerClassique>();
        movControl.GameData = GetComponentInParent<SaveData>();
        audioSource = GetComponent<AudioSource>();

        ShapesInitialize();
    }

    private void Start()
    {
        Instantiate();
    }


    ///////////////////////////////////// Methodes de la classe

    public override void Instantiate()
    {
        CreatePiece();
        PieceData data = movControl.piece;
        SpawnPiece(data);
    }

    public override void ShapesInitialize()
    {
        // Initialiser les donnees de chaque tetrimino ou triomino.
        for (int i = 0; i < triominoes.Length; i++)
        {
            triominoes[i].Initialize();
        }
    }

    // Generation des donnees d'une nouvelle piece
    public override void CreatePiece()
    {
        int random = Random.Range(0, triominoes.Length);
        IPieceData data = triominoes[random];
        movControl.piece = movControl.Initialize(data);
    }

    // Apparition de la piece genere
    public override void SpawnPiece(PieceData piece, bool reserve = false, int posY = 8)
    {
        spawnPosition.y = FindHightestPoint();
        spawnPosition.x = 0;
        TriominoData triominoData = (TriominoData)piece.data;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(triominoData.pieceSpawnSound);
        }

        // Verifier si la position de spawn est valide, sinon termine le jeu.
        if (IsValidPosition(movControl.piece, spawnPosition))
        {
            movControl.InstantiatePiece(piece, spawnPosition);
            Set(movControl.piece);
        }
        else
        {
            base.GameOver();
        }
    }

    private int FindHightestPoint()
    {
        RectInt bounds = Bounds;

        for (int y = bounds.yMax - 2; y > bounds.yMin + distanceSpawnFrom; y--)
        {
            if (!IsLineEmpty(y - distanceSpawnFrom))
            {
                return y;
            }
        }
        return bounds.yMin + distanceSpawnFrom;
    }

    /// <summary>
    /// Gestion suppression des lignes
    /// </summary>


    // Methode pour verifier et effacer les lignes completes.
    public override void CheckEmptyLines()
    {
        RectInt bounds = Bounds;
        List<int> rowsToClear = new List<int>();
        int linesToCleared = 0;

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            if (IsLineFull(row))
            {
                rowsToClear.Add(row);
                linesToCleared++;
            }
        }

        if (rowsToClear.Count > 0)
        {
            RemoveLines(rowsToClear);
            audioSource.PlayOneShot(lineSuppresion);
        }

        // Augmentation du score
        if (linesToCleared > 0)
        {
            // Appliquez le multiplicateur de points en fonction du nombre de lignes effacées en même temps.
            scoreMultiplier = 1.0f + (linesToCleared - 1) * 0.5f; // Par exemple, x1 pour une ligne, x1.5 pour deux lignes, x2 pour trois lignes, etc.

            int pointsEarned = linesToCleared * 10;
            pointsEarned = Mathf.RoundToInt(pointsEarned * scoreMultiplier);
            score.maxScore += pointsEarned;
            score.AddScore(pointsEarned);
        }
    }

    //Suppression lignes sans animation
    private void RemoveLines(List<int> rows)
    {
        RectInt bounds = Bounds;
        int colStart = bounds.xMin;
        int colEnd = bounds.xMax;

        foreach (int y in rows)
        {
            for (int x = colStart; x < colEnd; x++)
            {
                Vector3Int pos = new Vector3Int(x, y);
                tilemap.SetTile(pos, null);
            }
        }

        //Actualisation position
        EraseHoles(rows);
    }
}
