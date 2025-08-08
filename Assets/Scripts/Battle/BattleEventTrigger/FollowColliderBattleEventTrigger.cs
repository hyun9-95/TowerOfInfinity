using Cysharp.Threading.Tasks;
using UnityEngine;

public class FollowColliderBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessColliderEvent();
    }

    private async UniTask ProcessColliderEvent()
    {
        var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>
                (Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

        if (colliderTriggerUnit == null)
            return;

        if (colliderTriggerUnit.Model == null)
            colliderTriggerUnit.SetModel(new RangeTriggerUnitModel());

        var colliderTriggerUnitModel = colliderTriggerUnit.Model;
        colliderTriggerUnitModel.SetFlip(Model.Sender.IsFlipX);
        colliderTriggerUnitModel.SetFollowTarget(Model.Sender.Transform);
        colliderTriggerUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
        colliderTriggerUnitModel.SetOnEventHit(OnEventHit);
        colliderTriggerUnitModel.SetHitCount(Model.HitCount);

        colliderTriggerUnit.ShowAsync().Forget();
    }
}
