using System.Collections.Generic;
using UnityEngine;

public static class DataClassique
{
    // Calcul des valeurs cosinus et sinus pour une rotation de 90 degrés (utilisées dans la matrice de rotation).
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);

    // Matrice de rotation pour la rotation des pièces de 90 degrés.
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    // Dictionnaire contenant la disposition des cellules pour chaque type de tétrimino.
    public static readonly Dictionary<Triomino, Vector2Int[]> Cells = new Dictionary<Triomino, Vector2Int[]>()
    {
        // Chaque tétrimino est défini par un ensemble de coordonnées Vector2Int pour ses cellules.
        { Triomino.I, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Triomino.L, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, 1) } },
        { Triomino.J, new Vector2Int[] { new Vector2Int( -1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, 1) } },
    };

    // Définition des "wall kicks" pour le tétrimino I .
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        // Chaque rangée représente un ensemble d'ajustements pour une rotation spécifique.
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 1, 0), new Vector2Int(0, 1), new Vector2Int(0,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-1, 0), new Vector2Int(0,-1), new Vector2Int(0, 1) },
    };

    // Définition des "wall kicks" pour les autres tétriminos (J, L).
    private static readonly Vector2Int[,] WallKicksLJ = new Vector2Int[,] {
         //Comme pour WallKicksI, mais ces ajustements sont utilisés pour les autres tetrominos.
        //droite et bas
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0,-1), new Vector2Int(1, 0) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0,-1), new Vector2Int(0, 1), new Vector2Int(1, 0) },
    };

    // Dictionnaire global des "wall kicks" pour chaque type de tetromino.
    public static readonly Dictionary<Triomino, Vector2Int[,]> WallKicks = new Dictionary<Triomino, Vector2Int[,]>()
    {
        // Assignation des matrices de "wall kicks" pour chaque type de tetromino.
        { Triomino.I, WallKicksI },
        { Triomino.J, WallKicksLJ },
        { Triomino.L, WallKicksLJ },
    };
}