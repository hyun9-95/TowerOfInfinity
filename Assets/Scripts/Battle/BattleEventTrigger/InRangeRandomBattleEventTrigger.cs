using Cysharp.Threading.Tasks;
using UnityEngine;

public class InRangeRandomBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessInRangeRandomEvent();
    }

    private async UniTask ProcessInRangeRandomEvent()
    {
        Vector2 centerPosition = Model.Sender.Transform.position;
        int spawnCount = Mathf.Max(1, Model.SpawnCount);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPosition = GetRandomPositionInRange(centerPosition, Model.Range);
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.TriggerUnitPath, randomPosition, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, null, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }

            if (Model.SpawnInterval > 0 && i < spawnCount - 1)
                await UniTaskUtils.DelaySeconds(Model.SpawnInterval);
        }
    }

    private Vector2 GetRandomPositionInRange(Vector2 center, float range)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(0f, range);
        return center + randomDirection * randomDistance;
    }
}