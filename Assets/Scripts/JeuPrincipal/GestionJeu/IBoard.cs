using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public abstract class IBoard : MonoBehaviour
{
    public Tilemap tilemap { get; protected set; } // Tilemap pour afficher les pieces.
    public IPieceController movControl { get; protected set; }
    public SpriteRenderer grid; // recuperer le sprite renderer du grid pour automatiser la taille du board
    protected Vector3Int spawnPosition = new Vector3Int(0, 8, 0);

    public Score score;
    public int scoreToAdd { get; set; }

    public float scoreMultiplier { get; set; } = 1.0f;

    public GameObject floatingTextScore;
    public GameOverManager gameOverManager;

    // Calcul des limites du plateau de jeu.
    public Vector2Int boardSize
    {
        get
        {
            Vector2 size = grid.size;
            return new Vector2Int((int)size.x, (int)size.y);
        }
    }

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    //////////////////////////////////// Methodes a redefenir

    public abstract void ShapesInitialize();
    public abstract void Instantiate();
    public abstract void CreatePiece();
    public abstract void SpawnPiece(PieceData piece, bool reserve = false, int posY = 8);

    public abstract void CheckEmptyLines();

    ///////////////////////////////////// Methodes de la classe

    /// <summary>
    /// Methodes basiques pour le fonctionnement du jeu
    /// </summary>

    // Gestion de la fin du jeu.
    public virtual void GameOver()
    {
        gameOverManager.ShowGameOver();
    }

    // Methode pour placer une piece sur le plateau.
    public void Set(PieceData piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    // Methode pour retirer une piece du plateau.
    public void Clear(PieceData piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // Methode pour verifier que la piece n'est pas sur l'emplacement.
    public bool CheckPiecePos(PieceData piece, Vector3Int pos)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            if (tilePosition == pos)
                return false;
        }
        return true;
    }

    // Verifier si une position est valide pour placer une piece.
    public bool IsValidPosition(PieceData piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // Verifier si la position est hors limites ou occupee.
            if (!bounds.Contains((Vector2Int)tilePosition) || tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Gestion suppression des lignes
    /// </summary>

    protected void EraseHoles(List<int> rows)
    {
        RectInt bounds = Bounds;
        int colStart = bounds.xMin;
        int colEnd = bounds.xMax;

        int y = rows[0];
        int next = 0; // Décalage
        Vector3Int actualPos;
        Vector3Int abovePos;

        while (y < bounds.yMax - next)
        {
            // Vérifie si la ligne du dessus doit être copiée
            if (!IsLineEmpty(y + next))
            {
                for (int x = colStart; x < colEnd; x++) // Boucle à travers les colonnes
                {
                    actualPos = new Vector3Int(x, y);
                    abovePos = new Vector3Int(x, y + next);

                    // Pour éviter de dupliquer la pièce active
                    if (!CheckPiecePos(movControl.piece, abovePos))
                    {
                        tilemap.SetTile(actualPos, null);
                    }
                    else
                    {
                        CopyTile(actualPos, abovePos);
                    }
                }
                y++;
            }
            else
            {
                next++; // Incrémente le décalage si la ligne est vide
            }
        }
    }

    private void CopyTile(Vector3Int originalPos, Vector3Int copyPos)
    {
        TileBase copyTile = tilemap.GetTile(copyPos);
        tilemap.SetTile(originalPos, copyTile);
        tilemap.SetTile(copyPos, null);
    }

    public virtual IEnumerator ShowFloatingTextLine(int scoreGained, Color textColor, int heightY)
    {
        Vector3 startPosition = new Vector3(Bounds.xMin - 2, heightY, movControl.piece.position.z);
        Vector3 endPosition = new Vector3(-13, 0, 0);

        // Instancier le texte avec la position de départ
        GameObject scoreText = Instantiate(floatingTextScore, startPosition, Quaternion.identity, transform);
        TextMesh textMesh = scoreText.GetComponent<TextMesh>();
        textMesh.text = "+" + scoreGained.ToString();
        textMesh.color = textColor;

        float animationDuration = 1.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < animationDuration && !gameOverManager.gameOverActive)
        {
            float t = elapsedTime / animationDuration;
            scoreText.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        scoreText.transform.position = endPosition;
        Destroy(scoreText);
        score.maxScore += scoreGained;
        score.AddScore(scoreGained);
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsLineEmpty(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanMoveDirection(PieceData piece, Vector2Int direction, int threshold)
    {
        // Créez une copie temporaire de la position de la pièce
        Vector3Int newPosition = piece.position + new Vector3Int(direction.x, direction.y, 0) * threshold;

        // Vérifiez si la nouvelle position est valide
        bool valid = IsValidPosition(piece, newPosition);

        return valid;
    }
}
