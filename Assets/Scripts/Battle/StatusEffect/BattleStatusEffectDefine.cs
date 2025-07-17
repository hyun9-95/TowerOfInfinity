public struct BattleStatusEffectDefine
{
    public BattleStatusEffect CreateNewBattleStatusEffect(BattleEventType statusType)
    {
        switch (statusType)
        {
            default:
                Logger.Error($"정의되지 않은 BattleStatusEffect : {statusType}");
                break;
        }

        return null;
    }
}
