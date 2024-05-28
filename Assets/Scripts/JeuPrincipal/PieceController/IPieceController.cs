using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public abstract class IPieceController : MonoBehaviour
{
    // References et parametres de base de la piece.
    protected PieceData _piece = new PieceData();
    protected SaveData _gameData;
    protected IBoard _board;
    public List<GameObject> scoreTexts = new List<GameObject>();


    public IBoard board
    {
        get
        {
            if (this is PieceControllerRetro retroController)
            {
                return ((BoardRetro)this._board);
            }
            else if (this is PieceControllerClassique classiqueController)
            {
                return ((BoardClassique)this._board);
            }
            else
            {
                return ((BoardBonus)this._board);
            }
        }
        protected set {; _board = value; }
    }
    public PieceData piece
    {
        get
        {
            if (this is PieceControllerRetro retroController)
            {
                return ((PieceControllerRetro)this)._piece;
            }
            else
            {
                return ((PieceControllerClassique)this)._piece;
            }
        }
        set { _piece = value; }
    }
    public SaveData GameData
    {
        get
        {
            if (this is PieceControllerRetro retroController)
            {
                return ((PieceControllerRetro)this)._gameData;
            }
            else
            {
                return ((PieceControllerClassique)this)._gameData;
            }
        }
        set { _gameData = value; }
    }

    //////////////////////////////////// Methodes a redefinir

    public abstract void InstantiatePiece(PieceData pieceData, Vector3Int position);
    public abstract void HandleMoveInputs();
    public abstract bool Move(Vector2Int translation);


    //////////////////////////////////// Methodes communes

    public PieceData Initialize(IPieceData data)
    {
        //Init n'ajoute pas directement data à _piece pour gérer le cas où on a besoin d'init un PieceData mais ne pas le stocker directement (ex : preview)
        PieceData result = new PieceData();
        result.data = data;

        if (result.cells == null)
        {
            result.cells = new Vector3Int[result.data.cells.Length];
        }

        for (int i = 0; i < result.cells.Length; i++)
        {
            result.cells[i] = (Vector3Int)result.data.cells[i];
        }

        return result;
    }

    public void Rotate(int direction)
    {
        // Fait tourner la piece si possible, sinon fait des ajustements ("wall kicks").
        int originalRotation = piece.rotationIndex;

        // Faire pivoter toutes les cellules a l'aide d'une matrice de rotation
        piece.rotationIndex = Wrap(piece.rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Inverser la rotation si les tests echouent
        if (!TestWallKicks(piece.rotationIndex, direction))
        {
            piece.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        //On regarde si la piece est un trio ou tetra et en fonction on recupere la bonne matrice;
        float[] matrix;

        if (piece.data is TriominoData triomino)
        {
            matrix = DataClassique.RotationMatrix;
            ManageCoord(matrix, direction, false);
        }
        else
        {
            matrix = DataRetro.RotationMatrix;
            ManageCoord(matrix, direction, true);
        }
    }

    protected virtual void ManageCoord(float[] matrix, int direction, bool IsTetra)
    {
        //Gestion en fonction du type de forme (tetra ou trio) et de la forme (lettre)
        if (IsTetra)
        {
            //Si on a une piece tetra spéciale (I ou O) on aura une gestion spécifique sinon pareil pour les autres
            TetrominoData dataTetro = (TetrominoData)piece.data;

            if (dataTetro.tetromino == Tetromino.I || dataTetro.tetromino == Tetromino.O)
            {
                for (int i = 0; i < piece.cells.Length; i++)
                {
                    Vector3 cell = piece.cells[i];

                    int x, y;

                    // "I" et "O" sont tournes a partir d'un point central decale
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));

                    piece.cells[i] = new Vector3Int(x, y, 0);
                }
                return;
            }
        }

        //Triomino et Tetromino normal
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3 cell = piece.cells[i];

            int x, y;

            x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
            y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));

            piece.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        // Teste les ajustements de rotation pres des murs ("wall kicks").
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < piece.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = piece.data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        // Obtient l'index pour les ajustements de rotation dans le tableau de "wall kicks".
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, piece.data.wallKicks.GetLength(0));
    }

    private int Wrap(int index, int min, int max)
    {
        // Methode mathematique pour s'assurer que l'index se trouve au sein de la plage de valeurs (pour les rotations, par exemple).
        if (index < min)
        {
            return max - (min - index) % (max - min);
        }
        else
        {
            return min + (index - min) % (max - min);
        }
    }

    public IEnumerator ShowFloatingTextPiece(int scoreGained)
    {
        Vector3 startPosition = new Vector3(piece.position.x, piece.position.y + 1, piece.position.z);
        Vector3 endPosition = new Vector3(-13, 0, 0);

        // Instancier le texte avec la position de départ
        GameObject scoreText = Instantiate(board.floatingTextScore, startPosition, Quaternion.identity, transform);
        scoreText.GetComponent<TextMesh>().text = "+" + scoreGained.ToString();
        scoreTexts.Add(scoreText);

        float animationDuration = 1.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < animationDuration && !board.gameOverManager.gameOverActive)
        {
            float t = elapsedTime / animationDuration;
            scoreText.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        scoreText.transform.position = endPosition;
        Destroy(scoreText);
        board.score.AddScore(scoreGained);
    }
}
