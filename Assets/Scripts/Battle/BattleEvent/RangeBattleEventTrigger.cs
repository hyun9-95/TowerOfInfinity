#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RangeBattleEventTrigger : BattleEventTrigger
{
    public async override UniTask Process()
    {
        await ProcessRangeEvent();
    }

    private async UniTask ProcessRangeEvent()
    {
        if (!string.IsNullOrEmpty(Model.PrefabName))
        {
            var hitTargetEffect = await ObjectPoolManager.Instance.SpawnTimedMono<HitTargetRangeUnit>
            (Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (hitTargetEffect == null)
                return;

            if (hitTargetEffect.Model == null)
                hitTargetEffect.SetModel(new HitTargetRangeUnitModel());

            var hitTargetEffectUnitModel = hitTargetEffect.Model;
            hitTargetEffectUnitModel.SetRange(Model.Range);
            hitTargetEffectUnitModel.SetFlip(Model.Sender.IsFlipX);
            hitTargetEffectUnitModel.SetOnEventHit(OnEventHit);

            hitTargetEffect.ShowAsync().Forget();
        }
        // HitEffect만 있는 경우
        else if (!string.IsNullOrEmpty(Model.HitEffectPrefabName))
        {
            var colliders = Physics2D.OverlapCircleAll(Model.Sender.Transform.position, Model.Range, (int)LayerFlag.Character);

            if (colliders == null || colliders.Length == 0)
                return;

            colliders.SortByNearest(Model.Sender.Transform.position);

            foreach (var collider in colliders)
                OnEventHit(collider);
        }
    }

    protected override void OnComplete()
    {
        base.OnComplete();
    }
}
