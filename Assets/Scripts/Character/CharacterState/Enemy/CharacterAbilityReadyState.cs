using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterStates/Character/AbilityReady")]
public class CharacterAbilityReadyState : ScriptableCharacterState
{
    public override int Priority => 1;
    public override CharacterAnimState AnimState => CharacterAnimState.Ready;

    public override bool CheckEnterCondition(CharacterUnitModel model)
    {
        return model.ReadyCoolTime <= 0;
    }

    public override bool CheckExitCondition(CharacterUnitModel model)
    {
        return model.CurrentAnimState != CharacterAnimState.Ready;
    }

    public override void OnEnterState(CharacterUnitModel model)
    {
        var param = new BattleObserverParam();
        param.SetModelValue(model);
        ObserverManager.NotifyObserver(BattleObserverID.AbilityReady, param);
    }

    public override void OnExitState(CharacterUnitModel model)
    {
        model.SetReadyCoolTime(FloatDefine.DEFAULT_ABILITY_READY_COOLTIME);
    }

    public override void OnStateAction(CharacterUnitModel model)
    {
        model.ReduceReadyCoolTime();
    }
}