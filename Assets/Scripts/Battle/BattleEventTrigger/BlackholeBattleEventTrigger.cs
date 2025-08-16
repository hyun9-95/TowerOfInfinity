using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackholeBattleEventTrigger : BattleEventTrigger
{
    protected override async UniTask OnProcess()
    {
        await ProcessBlackhole();
    }

    private async UniTask ProcessBlackhole()
    {
        for (int i = 0; i < Model.SpawnCount; i++)
        {
            var blackholeUnit = await SpawnUnitAsync<BlackholeColliderTriggerUnit>
                (Model.TriggerUnitPath, Model.Sender.Transform.position, Quaternion.identity);

            if (blackholeUnit != null)
            {
                var model = BattleEventTriggerFactory.CreateBlackholeUnitModel(Model, OnEventHit);
                blackholeUnit.SetModel(model);
                blackholeUnit.ShowAsync().Forget();
            }

            if (Model.SpawnInterval > 0 && i < Model.SpawnCount - 1)
                await UniTaskUtils.DelaySeconds(Model.SpawnInterval);
        }
    }
}