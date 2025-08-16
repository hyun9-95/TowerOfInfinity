#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FrontAngleProjectileBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessFrontAngleProjectiles();
    }

    private async UniTask ProcessFrontAngleProjectiles()
    {
        float angleStep = GetAngleStep();
        float startAngle = GetStartAngle();

        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var projectileUnit = await SpawnUnitAsync<ProjectileTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (projectileUnit == null)
                continue;

            float currentAngle = startAngle + (angleStep * i);
            Vector3 direction = GetDirectionFromAngle(currentAngle);
            
            var model = BattleEventTriggerFactory.CreateProjectileUnitModel(Model, direction, null, OnEventHit);
            projectileUnit.SetModel(model);
            projectileUnit.ShowAsync().Forget();
        }
    }

    private float GetAngleStep()
    {
        if (Model.SpawnCount <= 1)
            return 0f;

        float totalAngle = 45f;
        return totalAngle / (Model.SpawnCount - 1);
    }

    private float GetStartAngle()
    {
        if (Model.SpawnCount <= 1)
            return 0f;

        float totalAngle = 45f;
        return -totalAngle / 2f;
    }

    private Vector3 GetDirectionFromAngle(float angle)
    {
        float baseAngle = Model.Sender.IsFlipX ? 180f : 0f;
        float finalAngle = (baseAngle + angle) * Mathf.Deg2Rad;
        
        return new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), 0f).normalized;
    }
}