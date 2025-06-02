#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public abstract class Ability
{
    protected AbilityModel Model { get; private set; }

    public void SetModel(AbilityModel model)
    {
        Model = model;
    }

    protected virtual void OnProcess()
    {
        BattleEventTriggerModel battleEventTriggerModel = Model.CreateTriggerModel();

        if (battleEventTriggerModel == null)
            return;

        BattleEventTrigger battleSkillTrigger = BattleEventTriggerFactory.Create(battleEventTriggerModel.TriggerType);
        battleSkillTrigger.SetModel(battleEventTriggerModel);
        battleSkillTrigger.Process().Forget();
    }
}
