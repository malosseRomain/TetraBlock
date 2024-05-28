using UnityEngine;
using UnityEngine.Tilemaps;

public class PutAside : MonoBehaviour
{

    public Tilemap tilemap { get; private set; }
    private PieceData piece;

    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    public void Initialize(PieceData piece)
    {
        tilemap.ClearAllTiles();
        this.piece = piece;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            tilemap.SetTile(piece.cells[i], piece.data.tile);
        }
    }

    public PieceData GetPieceData() { return piece; }
}
