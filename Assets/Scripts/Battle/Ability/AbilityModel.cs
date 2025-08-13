using System;

public class AbilityModel : IBaseUnitModel
{
    public DataAbility AbilityData { get; private set; }
    public CharacterUnitModel Owner { get; private set; }
    public int Level { get; private set; }
    public float CoolTime => abilityBalance.GetCoolTime(Owner.Level);

    public float Range => abilityBalance.GetRange(Owner.Level);

    private ScriptableAbilityBalance abilityBalance;

    public void SetByAbilityData(DataAbility ability, CharacterUnitModel owner)
    {
        if (ability.IsNullOrEmpty())
            return;

        abilityBalance = AbilityBalanceFactory.Instance.GetAbilityBalance(ability.Id);

        Owner = owner;
        AbilityData = ability;
    }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void LevelUp()
    {
        Level = Math.Min(++Level, IntDefine.MAX_ABILITY_LEVEL);
    }

    public BattleEventTriggerModel CreateTriggerModel()
    {
        BattleEventTriggerModel triggerModel = new BattleEventTriggerModel();
        triggerModel.Initialize(Owner, Level, AbilityData, abilityBalance);

        return triggerModel;
    }
}
