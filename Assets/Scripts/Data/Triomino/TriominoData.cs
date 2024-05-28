using UnityEngine;
using UnityEngine.Tilemaps;

public enum Triomino
{
    I, J, L
}


// Structure pour stocker les donnees specifiques a chaque tetrimino.
[System.Serializable]
public struct TriominoData : IPieceData
{
    public Triomino triomino;
    public AudioClip pieceSpawnSound;

    [SerializeField]
    private Tile _tile;

    public Tile tile
    {
        get { return _tile; }
    }

    // Proprietes pour acceder aux cellules et aux mouvements de rotation ("wall kicks").
    public Vector2Int[] cells { get; private set; } // Positions des cellules constituant le tetrimino.
    public Vector2Int[,] wallKicks { get; private set; } // Mouvements possibles lors de la rotation pres d'un mur.


    public void Initialize()
    {
        // Attribution des cellules et des mouvements de rotation en fonction du type de t�trimino.
        cells = DataClassique.Cells[triomino];
        wallKicks = DataClassique.WallKicks[triomino];

        //Generation du bonus ou malus
        // .....
    }
}
