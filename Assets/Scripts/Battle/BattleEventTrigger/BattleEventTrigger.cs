#pragma warning disable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEventTrigger
{
    protected BattleEventTriggerModel Model { get; private set; }

    public void SetModel(BattleEventTriggerModel skillInfoValue)
    {
        Model = skillInfoValue;
    }

    public async UniTask Process()
    {
        await OnProcess();

#if CHEAT
        if (Model.Range > 0)
            CheatManager.DrawWireSphereFromMainCharacter(Model.Range);
#endif
    }

    protected virtual async UniTask OnProcess() { }

    protected async UniTask<T> SpawnUnitAsync<T>(string prefabName, Vector3 position, Quaternion rotation) where T : PoolableMono, IBattleEventTriggerUnit
    {
        var unit = await ObjectPoolManager.Instance.SpawnPoolableMono<T>(prefabName, position, rotation);

        // 오브젝트 풀에서 재사용할 때 localScale값 리셋하므로 별도 리셋처리 필요 X
        if (Model.Scale > 0)
            unit.transform.localScale += unit.transform.localScale * Model.Scale;

        return unit;
    }

    private CharacterUnitModel GetTargetModel(Collider2D collider)
    {
        var targetModel = BattleSceneManager.GetCharacterModel(collider);

        // 죽으면 타겟으로 감지안함.
        if (targetModel != null && targetModel.Hp <= 0)
            return null;

        return targetModel;
    }

    protected void SendBattleEventToTarget(CharacterUnitModel targetModel, Vector3 hitPos, Collider2D hitTarget = null)
    {
        if (Model.BattleEventDatas.Count == 1)
        {
            SendBattleEvent(targetModel);
        }
        else
        {
            SendBattleEvents(targetModel);
        }

        if (hitPos != Vector3.zero && Model.HitForce > 0)
        {
            var hitForceDir = (hitTarget.transform.position - hitPos).normalized;
            targetModel.ActionHandler.OnAddForceAsync(hitForceDir, Model.HitForce).Forget();
        }

        if (hitTarget != null && !string.IsNullOrEmpty(Model.HitEffectPrefabName))
            OnSpawnHitEffect(hitTarget.transform.position).Forget();
    }

    private async UniTask OnSpawnHitEffect(Vector3 pos)
    {
        var hitEffect = await ObjectPoolManager.Instance.SpawnPoolableMono<TimedPoolableMono>
            (Model.HitEffectPrefabName, position: pos, Quaternion.identity);

        hitEffect.transform.localPosition += hitEffect.LocalPosOffset;
        hitEffect.Flip(hitEffect.transform.position.x < Model.Sender.Transform.position.x);
    }

    private void SendBattleEvent(CharacterUnitModel target)
    {
        var battleEventModel = Model.CreateBattleEventModel(target);
        target.BattleEventProcessor.ReceiveBattleEvent(battleEventModel);
    }

    private void SendBattleEvents(CharacterUnitModel target)
    {
        var battleEventModels = Model.CreateBattleEventModels(target);
        foreach (var battleEventModel in battleEventModels)
        {
            target.BattleEventProcessor.ReceiveBattleEvent(battleEventModel);
        }
    }

    private bool IsOverSendCount(int count)
    {
        // 0이면 횟수 제한이 없는 경우.
        if (Model.HitCount == 0)
            return false;

        return count >= Model.HitCount;
    }

    #region OnEvent
    protected bool OnEventHit(Collider2D hitTarget, Vector3 hitPos)
    {
        if (Model == null)
            return false;

        if (hitTarget == null)
            return false;

        var targetModel = GetTargetModel(hitTarget);

        if (targetModel == null)
            return false;

        if (targetModel.TeamTag != Model.TargetTeamTag)
            return false;

        SendBattleEventToTarget(targetModel, hitPos, hitTarget);

        return true;
    }

    protected Vector2 OnGetFixedDirection(DirectionType directionType)
    {
        Vector2 direction = directionType switch
        {
            DirectionType.Up => Vector2.up,
            DirectionType.Down => Vector2.down,
            DirectionType.Left => Vector2.left,
            DirectionType.Right => Vector2.right,
            DirectionType.Owner => Model.Sender.IsFlipX ? Vector2.left : Vector2.right,
            _ => Vector2.zero
        };

        return direction;
    }

    protected Vector2 GetRandomDirection()
    {
        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
    }

    protected HashSet<CharacterUnitModel> GetEnemiesInRange(int maxCount = 0)
    {
        Vector2 senderPosition = Model.Sender.Transform.position;
        var colliders = Physics2D.OverlapCircleAll(senderPosition, Model.Range, (int)LayerFlag.Character);
        var enemies = new HashSet<CharacterUnitModel>();
        
        colliders.SortByNearest(senderPosition);
        
        foreach (var collider in colliders)
        {
            if (maxCount > 0 && enemies.Count >= maxCount)
                break;
                
            // 카메라에 보이는 것만 1차 필터링
            if (!CameraManager.Instance.IsVisibleFromWorldCamera(collider.transform.position))
                continue;

            var targetModel = BattleSceneManager.GetCharacterModel(collider);

            if (targetModel == null || targetModel.TeamTag == Model.Sender.TeamTag)
                continue;

            if (targetModel == Model.Sender)
                continue;

            enemies.Add(targetModel);
        }
        
        return enemies;
    }

    #endregion
}
