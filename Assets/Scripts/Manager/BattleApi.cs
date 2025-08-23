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
        if (Model.OnDamage == null)
            return;

        Model.OnDamage(sender, receiver, value, damageType);
    }

    public static void OnHeal(CharacterUnitModel sender, CharacterUnitModel receiver, float value)
    {
        if (Model.OnHeal == null)
            return;

        Model.OnHeal(sender, receiver, value);
    }

    public static void OnExpGainRangeUp()
    {
        if (Model.OnExpGainRangeUp == null)
            return;

        Model.OnExpGainRangeUp();
    }

    public static CharacterUnitModel OnGetAliveCharModel(int instanceId)
    {
        if (Model.OnGetAliveCharModelById == null)
            return null;

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
        if (Model.OnCheatSpawnBoss == null)
            return;

        Model.OnCheatSpawnBoss();
    }

    public static void OnCheatLevelUp()
    {
        if (Model.OnCheatLevelUp == null)
            return;

        Model.OnCheatLevelUp();
    }

    public static void OnCheatLevelUpWithDraw(BattleCardTier tier = BattleCardTier.Common)
    {
        if (Model.OnCheatLevelUpWithDraw == null)
            return;

        Model.OnCheatLevelUpWithDraw(tier);
    }

    public static void OnCheatAddWave()
    {
        if (Model.OnCheatAddWave == null)
            return;

        Model.OnCheatAddWave();
    }
#endif
}