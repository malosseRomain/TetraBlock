using UnityEngine;

/// <summary>
/// Cette classe doit permettre de mieux gérer la transmission des données entre le jeu, la preview et la reserve
/// </summary>
public class PieceData
{
    public IPieceData data { get; set; }
    public Vector3Int[] cells { get; set; } //permet de stocker la position locale en Vector3Int[] plutôt qu'en Vector2Int (dans data)
    public Vector3Int position { get; set; }
    public int rotationIndex { get; set; }
}
