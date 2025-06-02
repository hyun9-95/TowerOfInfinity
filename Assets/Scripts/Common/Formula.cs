public static class Formula
{
    public static float GetNormalDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float attackValue)
    {
        float def = receiver.GetStatValue(StatType.Defense);
        float attack = attackValue;

        float damage = attack - def;

        if (damage <= 0)
            damage = 1;

        return damage;
    }

    public static void SendDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float attackValue)
    {
        receiver.ReduceHp(GetNormalDamage(sender, receiver, attackValue));
    }

    public static float GetExpGainRangeRadius(int level)
    {
        return 1 + (level * 0.1f);
    }
}
