using UnityEngine.Tilemaps;
using UnityEngine;

public enum BonusMalus
{
    bombe, slowmotion, multiplicateurScore, accelerate
}

[System.Serializable]
public struct EffectValue
{
    public Tile tile;
    public BonusMalus effet;
    [SerializeField]
    public plageVal plageProba;
    public float duration;
    public int multiplicateur;
}



public class Effect
{
    public EffectValue value;

    [HideInInspector]
    public Vector3Int position;

    [HideInInspector]
    public int relatedCell;

    public void ActivateEffect(BoardBonus board)
    {
        ComportementBonus comportement = board.gameObject.GetComponent<ComportementBonus>();

        switch (value.effet)
        {
            case BonusMalus.bombe:
                comportement.BombExplosion(position);
                break;

            case BonusMalus.slowmotion:
                comportement.ChangeSpeed(value.duration, value.multiplicateur, false);
                break;

            case BonusMalus.accelerate:
                comportement.ChangeSpeed(value.duration, value.multiplicateur, true);
                break;

            case BonusMalus.multiplicateurScore:
                comportement.ScoreMulti(value.duration, value.multiplicateur);
                break;

            default:
                Debug.Log("Effet non prévu");
                break;
        }
    }
}


[System.Serializable]
public class plageVal
{
    public float val1;
    public float val2;

    public plageVal(float val1, float val2)
    {
        this.val1 = val1;
        this.val2 = val2;
    }
}

