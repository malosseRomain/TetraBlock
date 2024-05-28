using UnityEngine;

public class PieceControllerClassique : IPieceController
{
    private new BoardClassique board;
    public Pause pause;
    // Delais pour les mouvements
    public float moveDelay = 0.1f;

    // Timers pour gerer les delais.
    private float moveTime;

    //Sounds
    private AudioPieceMovements audioPieceMovements;
    private bool isRadarDownSoundPlaying = false;
    private bool isRadarUpSoundPlaying = false;
    private bool isRadarLeftSoundPlaying = false;
    private bool isRadarRightSoundPlaying = false;


    /////////////////////////////////////////////////////// Event Functions

    private void Awake()
    {
        audioPieceMovements = GetComponent<AudioPieceMovements>();
        this.board = GetComponent<BoardClassique>();
        base.board = this.board;
    }

    private void Update()
    {
        if (board.gameOverManager.gameOverActive || pause.pauseActive)
            return;

        // Mise a jour de la position et de la rotation de la piece a chaque frame.
        board.Clear(piece);

        if (Input.GetKeyDown(GameData.DicKeyCode["RotGauche"]))
        {
            audioPieceMovements.PlayRotateSound();
            Rotate(-1);
        }
        else if (Input.GetKeyDown(GameData.DicKeyCode["RotDroite"]))
        {
            audioPieceMovements.PlayRotateSound();
            Rotate(1);
        }

        // Permet de maintenir les touches pour bouger mais sans que tout soit fait instantanement
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        Lock();

        board.Set(piece);
    }

    /////////////////////////////////////////////////////// Methodes de la classe

    // Initialisation de la piece avec ses donnees.
    public override void InstantiatePiece(PieceData pieceData, Vector3Int position)
    {
        piece.position = position;
        piece.rotationIndex = 0;

        moveTime = Time.time + moveDelay;
    }


    public override void HandleMoveInputs()
    {
        // Mouvement vertical
        if (Input.GetKeyDown(GameData.DicKeyCode["MovBas"]))
        {
            if (Move(Vector2Int.down))
                audioPieceMovements.PlayMoveDownSound();

            CheckContact(Vector2Int.down);
        }
        else if (Input.GetKeyDown(GameData.DicKeyCode["MovHaut"]))
        {
            if (Move(Vector2Int.up))
                audioPieceMovements.PlayMoveUpSound();

            CheckContact(Vector2Int.up);
        }

        // Mouvement latéral
        if (Input.GetKeyDown(GameData.DicKeyCode["MovGauche"]))
        {
            if (Move(Vector2Int.left))
                audioPieceMovements.PlayMoveLeftSound();

            CheckContact(Vector2Int.left);
        }
        else if (Input.GetKeyDown(GameData.DicKeyCode["MovDroite"]))
        {
            if (Move(Vector2Int.right))
                audioPieceMovements.PlayMoveRightSound();

            CheckContact(Vector2Int.right);
        }
    }

    private void CheckContact(Vector2Int direction)
    {
        //On déclenche un son spécifique si la pièce ne peut pas se déplacer dans la direction donnée
        if (!board.CanMoveDirection(piece, direction, 1))
        {
            if (direction == Vector2Int.down && !isRadarDownSoundPlaying)
            {
                isRadarDownSoundPlaying = true;
                audioPieceMovements.PlayRadarDownSound();
                isRadarDownSoundPlaying = false;
            }
            else if (direction == Vector2Int.up && !isRadarUpSoundPlaying)
            {
                isRadarUpSoundPlaying = true;
                audioPieceMovements.PlayRadarUpSound();
                isRadarUpSoundPlaying = false;
            }
            else if (direction == Vector2Int.left && !isRadarLeftSoundPlaying)
            {
                isRadarLeftSoundPlaying = true;
                audioPieceMovements.PlayRadarLeftSound();
                isRadarLeftSoundPlaying = false;
            }
            else if (direction == Vector2Int.right && !isRadarRightSoundPlaying)
            {
                isRadarRightSoundPlaying = true;
                audioPieceMovements.PlayRadarRightSound();
                isRadarRightSoundPlaying = false;
            }
        }


        // if (!board.CanMoveDirection(piece, direction, 1) && !isRadarDownSoundPlaying)
        // {
        //     isRadarDownSoundPlaying = true;
        //     audioPieceMovements.PlayRadarDownSound();
        //     isRadarDownSoundPlaying = false;
        // }
        // else if (!board.CanMoveDirection(piece, Vector2Int.up, 1) && !isRadarUpSoundPlaying)
        // {
        //     isRadarUpSoundPlaying = true;
        //     audioPieceMovements.PlayRadarUpSound();
        //     isRadarUpSoundPlaying = false;
        // }

        // if (!board.CanMoveDirection(piece, Vector2Int.left, 1) && !isRadarLeftSoundPlaying)
        // {
        //     isRadarLeftSoundPlaying = true;
        //     audioPieceMovements.PlayRadarLeftSound();
        //     isRadarLeftSoundPlaying = false;
        // }
        // else if (!board.CanMoveDirection(piece, Vector2Int.right, 1) && !isRadarRightSoundPlaying)
        // {
        //     isRadarRightSoundPlaying = true;
        //     audioPieceMovements.PlayRadarRightSound();
        //     isRadarRightSoundPlaying = false;
        // }
    }

    public void Lock()
    {
        if (Input.GetKeyDown(GameData.DicKeyCode["DescInstante"]) && !board.CanMoveDirection(piece, Vector2Int.down, 1))
        {
            int scorePiece = 4;
            board.score.maxScore += scorePiece;

            board.score.AddScore(scorePiece);
            audioPieceMovements.PlayLockSound();
            board.Set(piece);

            if (!board.gameOverManager.gameOverActive)
            {
                board.CreatePiece();
                board.SpawnPiece(piece);
            }
            board.CheckEmptyLines();
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
        }

        return valid;
    }
}