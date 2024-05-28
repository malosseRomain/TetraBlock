using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z
}

// Structure pour stocker les donnees specifiques a chaque tetrimino.
[System.Serializable]
public struct TetrominoData : IPieceData
{
    public Tetromino tetromino;

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
        // Attribution des cellules et des mouvements de rotation en fonction du type de tetrimino.
        cells = DataRetro.Cells[tetromino];
        wallKicks = DataRetro.WallKicks[tetromino];

        //Generation du bonus ou malus
        // .....
    }
}
