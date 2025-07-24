using Cysharp.Threading.Tasks;
using UnityEngine;

public class ColliderEnterBattleEventTrigger : BattleEventTrigger
{
    public async override UniTask Process()
    {
        await ProcessColliderEvent();
    }

    private async UniTask ProcessColliderEvent()
    {
        var colliderTriggerEnterUnit = await ObjectPoolManager.Instance.SpawnTimedMono<ColliderEnterTriggerUnit>
            (Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

        if (colliderTriggerEnterUnit == null)
            return;

        if (colliderTriggerEnterUnit.Model == null)
            colliderTriggerEnterUnit.SetModel(new RangeTriggerUnitModel());

        var colliderTriggerEnterUnitModel = colliderTriggerEnterUnit.Model;
        colliderTriggerEnterUnitModel.SetRange(Model.Range);
        colliderTriggerEnterUnitModel.SetFlip(Model.Sender.IsFlipX);
        colliderTriggerEnterUnitModel.SetFollowTarget(Model.Sender.Transform);
        colliderTriggerEnterUnitModel.SetOnEventHit(OnEventHit);

        colliderTriggerEnterUnit.ShowAsync().Forget();
    }
}
