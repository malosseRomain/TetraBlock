using UnityEngine;

public class PieceControllerBonus : PieceControllerRetro
{
    private new BoardBonus board;

    protected override void Awake()
    {
        board = GetComponent<BoardBonus>();
        base.board = this.board;
        base.Awake();
    }

    protected override void Update()
    {
        if (board.fallingEffect != null)
            board.ClearBonus(board.fallingEffect);

        base.Update();

        if (board.fallingEffect != null)
        {
            int cell = board.fallingEffect.relatedCell;
            board.fallingEffect.position = piece.position + piece.cells[cell];
            board.SetBonus(board.fallingEffect);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Effect effect = new Effect();
            effect.value = board.effects[3];
            effect.position = new Vector3Int();
            effect.position.x = 5;

            effect.ActivateEffect(board);
        }
    }


    public override bool Move(Vector2Int translation)
    {
        bool valid = base.Move(translation);

        if (valid && board.fallingEffect != null)
        {
            board.fallingEffect.position.x += translation.x;
            board.fallingEffect.position.y += translation.y;
            return true;
        }

        return valid;
    }

    protected override void Lock()
    {
        if (board.fallingEffect != null)
        {
            board.activeEffects.Add(board.fallingEffect);
            board.SetBonus(board.fallingEffect);
        }
        base.Lock();
    }
}
