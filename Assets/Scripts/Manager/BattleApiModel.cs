using System;
using UnityEngine;

public class BattleApiModel
{
    public Action<CharacterUnitModel, CharacterUnitModel, float, DamageType> OnDamage { get; private set; }
    public Action<CharacterUnitModel, CharacterUnitModel, float> OnHeal { get; private set; }
    public Action OnExpGainRangeUp { get; private set; }
    public Func<int, CharacterUnitModel> OnGetAliveCharModelById { get; private set; }

#if CHEAT
    public Action OnCheatSpawnBoss { get; private set; }
    public Action OnCheatLevelUp { get; private set; }
    public Action<BattleCardTier> OnCheatLevelUpWithDraw { get; private set; }
    public Action OnCheatAddWave { get; private set; }
#endif

    public void SetOnDamage(Action<CharacterUnitModel, CharacterUnitModel, float, DamageType> value)
    {
        OnDamage = value;
    }

    public void SetOnHeal(Action<CharacterUnitModel, CharacterUnitModel, float> value)
    {
        OnHeal = value;
    }

    public void SetOnExpGainRangeUp(Action value)
    {
        OnExpGainRangeUp = value;
    }

    public void SetOnGetAliveCharModelById(Func<int, CharacterUnitModel> value)
    {
        OnGetAliveCharModelById = value;
    }

#if CHEAT
    public void SetOnCheatSpawnBoss(Action value)
    {
        OnCheatSpawnBoss = value;
    }

    public void SetOnCheatLevelUp(Action value)
    {
        OnCheatLevelUp = value;
    }

    public void SetOnCheatLevelUpWithDraw(Action<BattleCardTier> value)
    {
        OnCheatLevelUpWithDraw = value;
    }

    public void SetOnCheatAddWave(Action value)
    {
        OnCheatAddWave = value;
    }
#endif
}