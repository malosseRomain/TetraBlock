using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BoardRetro : IBoard
{
    public Preview preview;
    public PutAside putAside;
    public Image cooldownIndicator;
    public TetrominoData[] tetrominoes;

    private bool reserveAvailable = true;
    private bool firstUse = true;
    public float DelayReserve = 2.0f;
    public int NbClignotementSuppr { get; protected set; } = 3;
    public float TimeClignotement { get; protected set; } = 0.2f;
    private bool cooldownActive = false;

    //hardDrop animation
    public float screenShakeDuration = 0.1f;
    public float screenShakeMagnitude = 0.3f;
    public bool isShaking = false;
    private Vector3 originalCameraPosition;

    //audio
    protected AudioSource audioSource;
    public AudioClip lineSuppresion;
    public AudioClip ReserveSound;

    ///////////////////////////////////// Event function

    protected virtual void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        movControl = GetComponent<PieceControllerRetro>();
        movControl.GameData = GetComponentInParent<SaveData>();
        audioSource = GetComponent<AudioSource>();      

        ShapesInitialize();
    }

    private void Start()
    {
        Instantiate();
    }

    ///////////////////////////////////// Methodes de la classe

    public override void GameOver()
    {
        base.GameOver();
        StartCoroutine(WaitAndStopAllCoroutines());
    }

    IEnumerator WaitAndStopAllCoroutines()
    {
        yield return new WaitForSeconds(0.1f);
        StopAllCoroutines();
        Time.timeScale = 0f;
    }

    public override void Instantiate()
    {
        CreatePiece();
        SpawnPiece(preview.pieceData);
    }

    public override void ShapesInitialize()
    {
        // Initialiser les donnees de chaque tetromino ou triomino.
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    // Generation d'une nouvelle piece dans la preview.
    public override void CreatePiece()
    {
        //On envoye directement les donnees a la preview pour ne pas stocker la meme valeur dans la piece active
        int random = Random.Range(0, tetrominoes.Length);
        IPieceData data = tetrominoes[random];
        preview.pieceData = movControl.Initialize(data);
    }

    // Recuperation de la piece depuis la preview ou la reserve pour la mettre dans le plateau
    public override void SpawnPiece(PieceData piece, bool reserve = false, int posY = 8)
    {
        spawnPosition.y = posY;
        spawnPosition.x = Random.Range(boardSize.x - 14, boardSize.x - 7);


        //Permet de gérer le cas où on spawn une pièce depuis la reserve
        if (!reserve)
            CreatePiece();


        // Verifier si la position de spawn est valide, sinon terminer le jeu.
        if (IsValidPosition(piece, spawnPosition))
        {
            movControl.InstantiatePiece(piece, spawnPosition);
            Set(piece);
        }
        else
        {
            GameOver();
        }
    }

    public IEnumerator ScreenShake()
    {
        // Sauvegardez la position de la caméra originale
        originalCameraPosition = Camera.main.transform.position;

        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < screenShakeDuration)
        {
            float xOffset = Random.Range(-screenShakeMagnitude, screenShakeMagnitude);
            float yOffset = Random.Range(-screenShakeMagnitude, screenShakeMagnitude);

            // Déplacez la caméra de manière aléatoire pour simuler la secousse
            Camera.main.transform.position = originalCameraPosition + new Vector3(xOffset, yOffset, originalCameraPosition.z);

            // Incrémentez le temps écoulé
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        // Réinitialisez la position de la caméra à sa position d'origine
        Camera.main.transform.position = originalCameraPosition;
        isShaking = false;
    }

    /// <summary>
    /// Gestion suppression des lignes
    /// </summary>

    // Methode pour verifier et effacer les lignes completes.
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

            //Suppression ligne
            StartCoroutine(RemoveLinesWithAnimation(rowsToClear));
            audioSource.PlayOneShot(lineSuppresion);


            // Augmentation du score
            // Appliquez le multiplicateur de points en fonction du nombre de lignes effacées en même temps.
            scoreMultiplier = 1.0f + (linesToCleared - 1) * 0.5f; // Par exemple, x1 pour une ligne, x1.5 pour deux lignes, x2 pour trois lignes, etc.

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
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, new Color(1.0f, 0.5f, 0.0f), movControl.piece.position.y));  //orange
                    break;

                case 4:
                    StartCoroutine(ShowFloatingTextLine(pointsEarned, Color.red, movControl.piece.position.y));
                    break;

                default:
                    break;
            }
        }
    }

    //Suppression lignes avec animation
    public virtual IEnumerator RemoveLinesWithAnimation(List<int> rows)
    {
        ((PieceControllerRetro)movControl).allowHardDropping = false; //Eviter que le score ce dedouble si on hard drop pendant l'animation removeTile
        RectInt bounds = Bounds;
        int colStart = bounds.xMin;
        int colEnd = bounds.xMax;

        List<List<Vector3Int>> tab = new List<List<Vector3Int>>();

        //Recuperation des tiles a suppr
        foreach (int row in rows)
        {
            List<Vector3Int> list = new List<Vector3Int>();

            for (int col = colStart; col < colEnd; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                list.Add(position);
                tilemap.SetTileFlags(position, TileFlags.None); // reinitialise les drapeaux des tuiles pour pouvoir changer de couleurs
            }
            tab.Add(list);
        }

        //Clignotement
        for (int i = 0; i < NbClignotementSuppr; i++)
        {
            SetColorTiles(tab, Color.clear); //Fade out
            yield return new WaitForSeconds(TimeClignotement);

            SetColorTiles(tab, Color.white); //Fade in
            yield return new WaitForSeconds(TimeClignotement);
        }

        //Suppression
        foreach (List<Vector3Int> row in tab)
        {
            foreach (Vector3Int pos in row)
            {
                tilemap.SetTile(pos, null);
            }
        }

        //Actualisation position
        EraseHoles(rows);
        ((PieceControllerRetro)movControl).allowHardDropping = true;
    }


    protected void SetColorTiles(List<List<Vector3Int>> tab, Color color)
    {
        foreach (List<Vector3Int> row in tab)
        {
            foreach (Vector3Int pos in row)
            {
                tilemap.SetColor(pos, color);
            }
        }
    }

    /// <summary>
    /// Reserve
    /// </summary>

    public void TryPutAside()
    {
        if (reserveAvailable && !((PieceControllerRetro)movControl).isHardDropping)
        {
            Clear(movControl.piece);

            if (firstUse)
            {
                firstUse = false;
                putAside.Initialize(movControl.piece);
                SpawnPiece(preview.pieceData, false, movControl.piece.position.y);// essaie de faire spawn a la position de la piece actuel pour pas avoir de probleme avec le verif de position
                StartCoroutine(DisableFunctionForSeconds(DelayReserve));
            }
            else if (IsValidPosition(putAside.GetPieceData(), putAside.GetPieceData().position))//permet de vérifier si la piece actuel peux etre remplacer par la piece du stockage
            {
                PieceData AsidePieceTemp = putAside.GetPieceData();
                putAside.Initialize(movControl.piece);
                SpawnPiece(AsidePieceTemp, true, AsidePieceTemp.position.y);
                StartCoroutine(DisableFunctionForSeconds(DelayReserve));
            }

            // Activer la barre de chargement et la remplir pendant le cooldown.
            StartCoroutine(FillCooldownIndicator(DelayReserve));
            audioSource.PlayOneShot(ReserveSound);
        }
    }

    protected IEnumerator FillCooldownIndicator(float cooldownTime)
    {
        if (!cooldownActive)
        {
            cooldownIndicator.fillAmount = 0.0f; // Démarrez avec la barre vide.
            cooldownActive = true;
        }

        float timer = 0.0f;
        Color startColor = Color.red;
        Color endColor = Color.green;

        while (timer < cooldownTime)
        {
            timer += Time.deltaTime;
            float progress = timer / cooldownTime;
            float fillAmount = Mathf.Clamp01(progress);
            cooldownIndicator.fillAmount = fillAmount;

            // Utilisez une fonction d'atténuation pour un dégradé de couleur plus doux.
            float easedProgress = EaseIn(progress);
            cooldownIndicator.color = Color.LerpUnclamped(startColor, endColor, easedProgress);

            yield return null;
        }
    }

    // Fonction d'atténuation EaseInOut pour une progression plus douce.
    float EaseIn(float t)
    {
        return Mathf.Pow(t, 2);
    }


    public void ResetCooldownIndicator()
    {
        cooldownActive = false;
    }

    protected IEnumerator DisableFunctionForSeconds(float disableTime)
    {
        reserveAvailable = false;

        yield return new WaitForSeconds(disableTime);

        reserveAvailable = true;
    }
}
