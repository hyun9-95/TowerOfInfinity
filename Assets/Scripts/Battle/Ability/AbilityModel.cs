public class AbilityModel : IBaseUnitModel
{
    public DataAbility AbilityData { get; private set; }
    public CharacterUnitModel Owner { get; private set; }
    public float CoolTime => abilityBalance.GetCoolTime(Owner.Level);

    private ScriptableAbilityBalance abilityBalance;

    public void SetByAbilityData(DataAbility ability, CharacterUnitModel owner)
    {
        if (ability.IsNull)
            return;

        abilityBalance = AbilityBalanceFactory.Instance.GetAbilityBalance(ability.Id);

        Owner = owner;
        AbilityData = ability;
    }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public BattleEventTriggerModel CreateTriggerModel()
    {
        BattleEventTriggerModel triggerModel = BattleEventTriggerFactory.CreateTriggerModel();
        triggerModel.Initialize(Owner, AbilityData, abilityBalance);

        return triggerModel;
    }
}
