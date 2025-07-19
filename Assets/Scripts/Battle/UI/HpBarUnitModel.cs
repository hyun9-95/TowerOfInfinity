using UnityEngine;

public class HpBarUnitModel : IBaseUnitModel
{
    public CharacterUnitModel Owner { get; private set; }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public float GetHpBarValue()
    {
        if (Owner == null)
            return 0;

        return Owner.Hp / Owner.GetStatValue(StatType.MaxHp, StatReferenceCondition.CurrentStat);
    }
}
