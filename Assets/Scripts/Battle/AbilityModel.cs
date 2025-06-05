public class AbilityModel : IBaseUnitModel
{
    public int AbilityDataId { get; private set; }
    public int Level { get; private set; }
    public CharacterUnitModel Owner { get; private set; }
    public AbilityBalance AbilityBalance { get; private set; }
    public float Value => AbilityBalance.GetValue(Level);
    public float CoolTime => AbilityBalance.GetCoolTime(Level);
    public float Speed => AbilityBalance.GetSpeed(Level);
    public float Range => AbilityBalance.GetRange(Level);
    public int TargetCount => AbilityBalance.GetTargetCount(Level);
    public StatType AffectStat => AbilityBalance.GetAffectStat();

    private DataBattleEvent battleEventData;

    public void SetByAbilityData(DataAbility ability, CharacterUnitModel owner, AbilityBalance abilityBalance)
    {
        if (ability.IsNull)
            return;

        battleEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)ability.BattleEvent);

        Owner = owner;
        AbilityDataId = ability.Id;
        AbilityBalance = abilityBalance;
    }

    public void SetOwner(CharacterUnitModel owner)
    {
        Owner = owner;
    }

    public BattleEventTriggerModel CreateTriggerModel()
    {
        BattleEventTriggerModel triggerModel = BattleEventTriggerFactory.CreateTriggerModel();
        triggerModel.SetSkillInfoByData(battleEventData);
        triggerModel.SetBalance(AbilityBalance, Level);
        triggerModel.SetSender(Owner);

        return triggerModel;
    }
}
