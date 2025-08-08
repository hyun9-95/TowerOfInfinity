using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InRangeFollowBattleEventTrigger : BattleEventTrigger
{
    public async override UniTask Process()
    {
        await ProcessInRangeFollowEvent();
    }

    private async UniTask ProcessInRangeFollowEvent()
    {
        var enemiesInRange = GetEnemiesInRange();
        
        if (enemiesInRange.Count == 0)
            return;

        int spawnCount = Mathf.Min(Model.SpawnCount, enemiesInRange.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Transform targetEnemy = enemiesInRange[i];
            
            var colliderTriggerUnit = await SpawnUnitAsync<ColliderTriggerUnit>
                (Model.PrefabName, Model.Sender.Transform.position, Quaternion.identity);

            if (colliderTriggerUnit == null)
                continue;

            if (colliderTriggerUnit.Model == null)
                colliderTriggerUnit.SetModel(new RangeTriggerUnitModel());

            var colliderTriggerUnitModel = colliderTriggerUnit.Model;
            colliderTriggerUnitModel.SetFlip(Model.Sender.IsFlipX);
            colliderTriggerUnitModel.SetFollowTarget(targetEnemy);
            colliderTriggerUnitModel.SetDetectTeamTag(Model.Sender.TeamTag.Opposite());
            colliderTriggerUnitModel.SetOnEventHit(OnEventHit);

            colliderTriggerUnit.ShowAsync().Forget();
        }
    }

    private List<Transform> GetEnemiesInRange()
    {
        Vector2 senderPosition = Model.Sender.Transform.position;
        var colliders = Physics2D.OverlapCircleAll(senderPosition, Model.Range, (int)LayerFlag.Character);
        var enemies = new List<Transform>();
        
        foreach (var collider in colliders)
        {
            var characterUnit = collider.GetComponent<CharacterUnit>();
            if (characterUnit != null && characterUnit.Model.TeamTag == Model.Sender.TeamTag.Opposite())
            {
                enemies.Add(collider.transform);
            }
        }
        
        return enemies;
    }
}