using UnityEngine;

public class PieceControllerRetro : IPieceController
{
    protected new BoardRetro board;
    public Pause pause;
    // Delais pour les mouvements et le verrouillage de la piece.
    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.3f;

    // Timers pour gerer les delais.
    protected float stepTime;
    protected float moveTime;
    protected float lockTime;

    public int scoreGained;

    public bool isHardDropping { get;  set; } = false;
    public bool allowHardDropping { get; set; } = true;
    protected bool putAsideActivated = true;

    //audio
    protected AudioSource audioSource;
    public AudioClip RotateSound;
    public AudioClip ShakeSound;

    /////////////////////////////////////////////////////// Event Functions
    protected virtual void Awake()
    {
        board = GetComponent<BoardRetro>();
        audioSource = board.GetComponent<AudioSource>();
        base.board = this.board;
    }

    protected virtual void Update()
    {
        if (board.gameOverManager.gameOverActive || pause.pauseActive)
        {
            return;
        }
        // Mise a jour de la position et de la rotation de la piece a chaque frame.
        board.Clear(piece);

        // Nous utilisons une minuterie pour permettre au joueur de faire des ajustements a la piece
        if (isHardDropping)
        {
            lockTime = lockDelay;
        }
        else
        {
            lockTime += Time.deltaTime;
            // Gestion de la rotation
            if (Input.GetKeyDown(GameData.DicKeyCode["RotGauche"]))
            {
                audioSource.PlayOneShot(RotateSound);
                Rotate(-1);
            }
            else if (Input.GetKeyDown(GameData.DicKeyCode["RotDroite"]))
            {
                audioSource.PlayOneShot(RotateSound);
                Rotate(1);
            }

            // Permet de maintenir les touches pour bouger mais sans que tout soit fait instantanement
            if (Time.time > moveTime)
            {
                HandleMoveInputs();
            }
        }

        if (Input.GetKeyDown(GameData.DicKeyCode["DescInstante"]) && allowHardDropping)
        {
            HardDrop();
            audioSource.PlayOneShot(ShakeSound);
        }

        // Deplace la piece a la prochaine ligne toutes les x secondes
        if (Time.time > stepTime)
        {
            Step();
        }

        //Reserve
        if (putAsideActivated && Input.GetKeyDown(GameData.DicKeyCode["PlacReserve"]))
        {
            board.TryPutAside();
        }

        board.Set(piece);
    }

    /////////////////////////////////////////////////////// Methodes de la classe

    // Initialisation de la piece avec ses donnees.
    public override void InstantiatePiece(PieceData pieceData, Vector3Int position)
    {
        piece = pieceData;
        piece.position = position;
        piece.rotationIndex = 0;

        //A chaque ajout d'une piece on recupere la nouvelle vitesse de chute si la difficulte est active
        DifficultyManager difficulte = gameObject.GetComponent<DifficultyManager>();
        if (difficulte)
            stepDelay = difficulte.GetSpeed();

        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
    }


    public override void HandleMoveInputs()
    {
        // chute douce
        if (Input.GetKey(GameData.DicKeyCode["MovBas"]) && Move(Vector2Int.down))
        {
            // Mettre a jour le temps de pas pour eviter le double mouvement
            stepTime = Time.time + stepDelay;
        }

        // mouvement lateraux
        if (Input.GetKey(GameData.DicKeyCode["MovGauche"]))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(GameData.DicKeyCode["MovDroite"]))
        {
            Move(Vector2Int.right);
        }
    }

    protected void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        // Une fois que la piece est inactive depuis trop longtemps, elle devient verrouillee
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    protected void HardDrop()
    {
        if (pause.pauseActive)
            return;
        stepTime = 0;
        isHardDropping = true;
        stepDelay = 0.02f;
    }

    protected virtual void Lock()
    {
        if (isHardDropping && !board.isShaking)
        {
            StartCoroutine(board.ScreenShake());
        }

        board.score.maxScore += scoreGained;
        // Appeler la coroutine pour l'animation du score
        StartCoroutine(ShowFloatingTextPiece(scoreGained));

        // Verrouille la piece en place, efface les lignes completes et fait spawn une nouvelle piece.
        board.Set(piece);
        board.CheckEmptyLines();
        isHardDropping = false;
        stepDelay = 1f;
        if (!board.gameOverManager.gameOverActive)
        {
            board.SpawnPiece(board.preview.pieceData);
        }
    }

    public override bool Move(Vector2Int translation)
    {
        // Deplace la piece si la nouvelle position est valide.
        Vector3Int newPosition = piece.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(piece, newPosition);

        // Enregistrer le mouvement uniquement si la nouvelle position est valide
        if (valid)
        {
            piece.position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }
}