using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BoardBonus : BoardRetro
{
    public Tilemap tilemapBonus { get; protected set; } // Tilemap pour afficher les bonus
    public EffectValue[] effects; // Liste initiale des effets
    public float ProbaBonus;
    public Effect fallingEffect { get; protected set; } = new Effect();
    public List<Effect> activeEffects { get; set; } = new List<Effect>(); // Liste des cases spéciales en jeu

    protected override void Awake()
    {
        base.Awake();
        tilemapBonus = transform.GetChild(1).gameObject.GetComponent<Tilemap>();
    }

    public override void SpawnPiece(PieceData piece, bool reserve = false, int posY = 8)
    {
        base.SpawnPiece(piece, reserve, posY);
        GenerateBonus(piece);
    }

    public override void CheckEmptyLines()
    {
        RectInt bounds = Bounds;
        List<int> rowsToClear = new List<int>();

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            if (IsLineFull(row))
            {
                rowsToClear.Add(row);
            }
        }
        if (rowsToClear.Count > 0)
        {
            int linesToCleared = rowsToClear.Count;
            StartCoroutine(RemoveLinesWithAnimation(rowsToClear));
            audioSource.PlayOneShot(lineSuppresion);


            // Appliquez le multiplicateur de points en fonction du nombre de lignes effacées en même temps
            scoreMultiplier = 1.0f + (linesToCleared - 1) * 0.5f; // Par exemple, x1 pour une ligne, x1.5 pour deux lignes, etc.
            int pointsEarned = linesToCleared * 10;
            pointsEarned = Mathf.RoundToInt(pointsEarned * scoreMultiplier);
            switch (linesToCleared)
            {
                case 1:
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, Color.green, movControl.piece.position.y));
                    break;
                case 2:
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, Color.yellow, movControl.piece.position.y));
                    break;
                case 3:
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, new Color(1.0f, 0.5f, 0.0f), movControl.piece.position.y));
                    break; // Orange
                case 4:
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, Color.red, movControl.piece.position.y));
                    break;
            }
        }
    }

    public void GenerateBonus(PieceData pieceData)
    {
        fallingEffect = null; // Reset bonus
        float randomValue = Random.Range(0, 100);

        // Gère la probabilité de ne pas avoir de bonus
        if (randomValue > ProbaBonus * 100) return;
        fallingEffect = new Effect();
        int randomPos = Random.Range(0, pieceData.data.cells.Length); // Savoir où le placer
        float randomEffect = Random.Range(0, 100); // Savoir quel effet choisir

        for (int i = 0; i < effects.Length; i++)
        {
            if (randomEffect > effects[i].plageProba.val1 * 100 && randomEffect <= effects[i].plageProba.val2 * 100)
            {
                fallingEffect.value = effects[i];
                break;
            }
        }

        // Position liée à une cellule
        fallingEffect.relatedCell = randomPos;
        fallingEffect.position = pieceData.cells[randomPos] + movControl.piece.position;
    }

    public void ClearBonus(Effect bonus)
    {
        Vector3Int pos = bonus.position;
        tilemapBonus.SetTile(pos, null);
    }

    public void SetBonus(Effect bonus)
    {
        Vector3Int pos = bonus.position;
        Tile tile = bonus.value.tile;
        tilemapBonus.SetTile(pos, tile);
    }

    public void MoveBonus(List<int> rows)
    {
        Effect effect;

        for (int i = 0; i < activeEffects.Count; i++)
        {
            effect = activeEffects[i];

            // Bonus à déclencher
            if (rows.Contains(effect.position.y))
                RemoveBonus(i--);


            // Bonus à descendre
            if (effect.position.y > rows.Min())
            {
                //on regarde le nombre de lignes continues a suppr en dessous du bonus
                int nbLignesDessous = 1;

                for (int j = 0; j < rows.Count - 1; j++)
                    if (rows[j + 1] - rows[j] == 1)
                        nbLignesDessous++;

                ClearBonus(activeEffects[i]);
                activeEffects[i].position.y -= nbLignesDessous;
                SetBonus(activeEffects[i]);
            }
        }
    }

    public void RemoveBonus(int index)
    {
        Effect effect = activeEffects[index];
        activeEffects.RemoveAt(index);
        ClearBonus(effect);
        effect.ActivateEffect(this);
    }

    public override IEnumerator RemoveLinesWithAnimation(List<int> rows)
    {
        yield return StartCoroutine(base.RemoveLinesWithAnimation(rows));
        MoveBonus(rows);
    }

    public IEnumerator RemoveColumnWithAnimation(int col, int row, AnimatedTile animationBomb)
    {
        RectInt bounds = Bounds;

        List<Vector3Int> list = new List<Vector3Int>();

        for (int line = bounds.yMin; line < bounds.yMax; line++)
        {
            Vector3Int position = new Vector3Int(col, line, 0);
            list.Add(position);
            tilemap.SetTileFlags(position, TileFlags.None); // reinitialise les drapeaux des tuiles pour pouvoir changer de couleurs 
        }


        //Animation
        int indOriginalRow = row + bounds.yMax; //Pour avoir l'indice de la ligne qui a la bombe qui se declenche on fait ce calcul car on sait que ind(0) = bounds.yMin et ind(list.Count-1) = yMax
        int cpt = 0;

        while (indOriginalRow - cpt >= 0 || indOriginalRow + cpt < list.Count)
        {
            if (indOriginalRow + cpt < list.Count)
                tilemapBonus.SetTile(list[indOriginalRow + cpt], animationBomb); //au dessus


            if (indOriginalRow - cpt >= 0)
                tilemapBonus.SetTile(list[indOriginalRow - cpt], animationBomb); //en dessous


            cpt++;
            yield return new WaitForSeconds(0.1f);
        }

        //Suppression
        foreach (Vector3Int pos in list)
        {
            tilemap.SetTile(pos, null);
            tilemapBonus.SetTile(pos, null);
        }


        //Gestion declenche bonus par bombe
        for (int i = 0; i < activeEffects.Count; i++)
        {
            if (activeEffects[i].position.x == col)
                RemoveBonus(i);
        }
    }
}