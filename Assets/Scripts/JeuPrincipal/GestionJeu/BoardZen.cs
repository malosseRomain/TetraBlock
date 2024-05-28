using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BoardZen : BoardRetro
{
    public int LignesASuppr = 5;
    private bool CanSupprLine = true;
    public bool SpawnPieceIsUsable = true;

    public override void SpawnPiece(PieceData piece, bool reserve = false, int posY = 8)
    {
        if (!reserve)
            spawnPosition.x = Random.Range(boardSize.x - 14, boardSize.x - 7);
        else
            spawnPosition.x = piece.position.x;

        spawnPosition.y = posY;

        movControl.InstantiatePiece(piece, spawnPosition);

        //Permet de g�rer le cas o� on spawn une pi�ce depuis la reserve
        if (!reserve && SpawnPieceIsUsable)
            CreatePiece();

        if (IsValidPosition(piece, spawnPosition) && CanMoveDirection(piece, Vector2Int.down, 2) && SpawnPieceIsUsable)
        {
            Set(piece);
        }
        else
        {
            List<int> rowsToClear = new List<int>();
            RectInt bounds = Bounds;

            for (int row = bounds.yMin; row < bounds.yMin + LignesASuppr; row++)
            {
                rowsToClear.Add(row);
            }
            StartCoroutine(RemoveLines(rowsToClear));
            audioSource.PlayOneShot(lineSuppresion);
        }
    }

    private IEnumerator RemoveLines(List<int> rows)
    {
        RectInt bounds = Bounds;
        int colStart = bounds.xMin;
        int colEnd = bounds.xMax;

        int next = rows.Count;
        int firstLine = rows[0];

        for (int y = 0; y < 10; y++)
        {
            if (CanSupprLine == true)
            {
                for (int x = colStart; x < colEnd; x++)
                {
                    Vector3Int pos = new Vector3Int(x, bounds.yMin);
                    tilemap.SetTile(pos, null);
                    yield return null;

                }
            }
            List<int> row = new List<int> { rows[0] };

            EraseHoles(row);
        }
    }
}
