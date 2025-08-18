using UnityEngine;

public class HpBarUnitModel : IBaseUnitModel
{
    public CharacterUnitModel Owner { get; private set; }

    public Camera BattleUICamera { get; private set; }

    public RectTransform BattleUICanvasRect { get; private set; }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public void SetBattleUICamera(Camera battleUICamera)
    {
        BattleUICamera = battleUICamera;
    }

    public void SetBattleUICanvas(RectTransform battleUICanvasRect)
    {
        BattleUICanvasRect = battleUICanvasRect;
    }

    public float GetHpBarValue()
    {
        if (Owner == null)
            return 0;

        return Owner.Hp / Owner.GetStatValue(StatType.MaxHp, StatReferenceCondition.CurrentStat);
    }
}
