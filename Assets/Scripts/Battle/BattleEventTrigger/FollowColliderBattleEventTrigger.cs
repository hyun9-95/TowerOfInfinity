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
        var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);
        
        if (colliderTriggerUnit != null)
        {
            var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, Model.Sender.Transform, OnEventHit);
            colliderTriggerUnit.SetModel(model);
            colliderTriggerUnit.ShowAsync().Forget();
        }
    }
}
