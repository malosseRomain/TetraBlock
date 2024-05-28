using UnityEngine;
using UnityEngine.Tilemaps;


public interface IPieceData
{
    Tile tile { get; }

    // Proprietes pour acceder aux cellules et aux mouvements de rotation ("wall kicks").
    Vector2Int[] cells { get; } // Positions des cellules constituant le tetrimino.
    Vector2Int[,] wallKicks { get; } // Mouvements possibles lors de la rotation pres d'un mur.

    void Initialize();
}