using UnityEngine;
using UnityEngine.Tilemaps;

public class Preview : MonoBehaviour
{
    private PieceData _piece;
    public Tilemap tilemap { get; private set; }
    public PieceData pieceData
    {
        get
        {
            return _piece;
        }

        set
        {
            _piece = value;

            tilemap.ClearAllTiles();

            for (int i = 0; i < _piece.cells.Length; i++)
            {
                tilemap.SetTile(_piece.cells[i], _piece.data.tile);
            }
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }
}
