using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public IBoard mainBoard;
    public IPieceController controller;
    private PieceData trackingPiece;

    // Tilemap pour afficher le fantome et les informations de position des cellules.
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        // Initialisation de la tilemap et allocation de l'espace pour les cellules.
        tilemap = GetComponentInChildren<Tilemap>();
        if (mainBoard is BoardRetro boardRetro)
        {
            cells = new Vector3Int[4];
        }
        else
        {
            cells = new Vector3Int[3];
        }
    }

    private void LateUpdate()
    {
        // Mise a jour du fantome 
        trackingPiece = controller.piece;

        Clear();   // Efface le fantome precedent.
        Copy();    // Copie la position des cellules de la piece suivie.
        Drop();    // Calcule position du fantome.
        Set();     // Le place a la nouvelle position.

    }

    private void Clear()
    {
        // Efface les tile du fantome de la tilemap.
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        // Copie les cellules de la piece suivie dans le fantome.
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        // Calcule la position la plus basse ou le fantome peut etre place sans collision.
        Vector3Int position = trackingPiece.position;

        int current = position.y;
        int bottom = -mainBoard.boardSize.y / 2 - 1;

        mainBoard.Clear(trackingPiece);

        // Boucle pour trouver la position la plus basse valide.
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (mainBoard.IsValidPosition(trackingPiece, position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        mainBoard.Set(trackingPiece);
    }

    private void Set()
    {
        // Place le fantome a la position calculee.
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

}
