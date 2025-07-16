public class AbilityModel : IBaseUnitModel
{
    public DataAbility AbilityData { get; private set; }
    public CharacterUnitModel Owner { get; private set; }

    public float CoolTime => AbilityData.CoolTime[Owner.Level];

    private DataBattleEvent battleEventData;

    public void SetByAbilityData(DataAbility ability, CharacterUnitModel owner)
    {
        if (ability.IsNull)
            return;

        battleEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)ability.BattleEvent);

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
        triggerModel.SetInfoByData(AbilityData, Owner.Level);
        triggerModel.SetSender(Owner);

        return triggerModel;
    }
}
