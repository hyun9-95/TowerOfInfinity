using UnityEngine;

public static class Formula
{
    public static int GetDamageAmount(CharacterUnitModel sender, CharacterUnitModel receiver, float attackValue, DamageType damageType)
    {
        float def = receiver.GetStatValue(StatType.Defense);
        float attack = attackValue;

        float damage = attack - def;

        if (damage <= 0)
            damage = 1;

        // 반올림
        return Mathf.RoundToInt(damage);
    }

    public static float GetExpGainRangeRadius(int level)
    {
        return 1 + (level * 0.1f);
    }
}
