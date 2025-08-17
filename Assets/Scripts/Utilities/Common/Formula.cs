using UnityEngine;

public static class Formula
{
    public static int GetDamageAmount(CharacterUnitModel sender, CharacterUnitModel receiver, float attackValue, DamageType damageType)
    {
        float def = receiver.GetStatValue(StatType.Defense);
        float attack = attackValue;

        float damage = attack - def;

        if (damage <= 0)
            damage = BattleDefine.MIN_DAMAGE_VALUE;

        // 반올림
        return Mathf.RoundToInt(damage);
    }

    public static float GetExpGainRangeRadius(int level)
    {
        return BattleDefine.EXP_RANGE_BASE_RADIUS + (level * BattleDefine.EXP_RANGE_LEVEL_MULTIPLIER);
    }
}
