using Cysharp.Threading.Tasks;
using UnityEngine;

public class OrbitBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessOrbitTriggers();
    }

    private async UniTask ProcessOrbitTriggers()
    {
        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var orbitUnit = await SpawnUnitAsync<OrbitTriggerUnit>(Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (orbitUnit == null)
                continue;

            float angleOffset = (BattleDefine.FULL_CIRCLE_DEGREES / Model.SpawnCount) * i;
            
            // 위에서 부터
            angleOffset += BattleDefine.QUARTER_CIRCLE_DEGREES;
            
            var model = BattleEventTriggerFactory.CreateOrbitUnitModel(Model, angleOffset, Model.Sender.Transform, OnEventHit);
            orbitUnit.SetModel(model);
            orbitUnit.ShowAsync().Forget();

            if (Model.SpawnInterval > 0 && i < Model.SpawnCount - 1)
                await UniTaskUtils.DelaySeconds(Model.SpawnInterval);
        }
    }
}