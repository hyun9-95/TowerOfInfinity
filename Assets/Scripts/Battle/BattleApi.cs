using UnityEngine;

public static class BattleApi
{
    private static BattleApiModel Model = new BattleApiModel();

    public static void InitializeModel(BattleApiModel model)
    {
        Model = model;
    }

    public static void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType = DamageType.Normal)
    {
        Model.OnDamage(sender, receiver, value, damageType);
    }

    public static void OnHeal(CharacterUnitModel sender, CharacterUnitModel receiver, float value)
    {
        Model.OnHeal(sender, receiver, value);
    }

    public static void OnExpGainRangeUp()
    {
        Model.OnExpGainRangeUp();
    }

    public static CharacterUnitModel OnGetAliveCharModel(int instanceId)
    {
        return Model.OnGetAliveCharModelById(instanceId);
    }

    public static CharacterUnitModel OnGetAliveCharModel(Transform transform)
    {
        return OnGetAliveCharModel(transform.gameObject.GetInstanceID());
    }

    public static CharacterUnitModel OnGetAliveCharModel(GameObject gameObject)
    {
        return OnGetAliveCharModel(gameObject.GetInstanceID());
    }

    public static CharacterUnitModel OnGetAliveCharModel(Collider2D collider)
    {
        return OnGetAliveCharModel(collider.gameObject.GetInstanceID());
    }

#if CHEAT
    public static void OnCheatSpawnBoss()
    {
        Model.OnCheatSpawnBoss();
    }

    public static void OnCheatLevelUp()
    {
        Model.OnCheatLevelUp();
    }

    public static void OnCheatLevelUpWithDraw(BattleCardTier tier = BattleCardTier.Common)
    {
        Model.OnCheatLevelUpWithDraw(tier);
    }

    public static void OnCheatAddWave()
    {
        Model.OnCheatAddWave();
    }
#endif
}