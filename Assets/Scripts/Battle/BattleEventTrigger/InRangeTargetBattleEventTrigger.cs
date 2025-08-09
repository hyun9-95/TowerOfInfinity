using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InRangeTargetBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessRandomRangeEvent();
    }

    private async UniTask ProcessRandomRangeEvent()
    {
        int spawnCount = Model.SpawnCount;
        var targetPositions = GetTargetPositions(spawnCount);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 targetPosition = targetPositions[i];
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>(Model.PrefabName, targetPosition, Quaternion.identity);

            if (colliderTriggerUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateColliderUnitModel(Model, null, OnEventHit);
                colliderTriggerUnit.SetModel(model);
                colliderTriggerUnit.ShowAsync().Forget();
            }
        }
    }

    private Vector2[] GetTargetPositions(int count)
    {
        Vector2 senderPosition = Model.Sender.Transform.position;
        var positions = new Vector2[count];
        
        var enemiesInRange = GetEnemyTransformsInRange();
        
        for (int i = 0; i < count; i++)
        {
            if (i < enemiesInRange.Count)
            {
                positions[i] = enemiesInRange[i].position;
            }
            else
            {
                positions[i] = GetRandomPositionInRange(senderPosition);
            }
        }
        
        return positions;
    }

    private Vector2 GetRandomPositionInRange(Vector2 centerPosition)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(0f, Model.Range);
        
        return centerPosition + randomDirection * randomDistance;
    }
}