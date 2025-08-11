using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;


public class ColliderTriggerUnit : PoolableBaseUnit<BattleEventTriggerUnitModel>, IBattleEventTriggerUnit, IObserver
{
    [SerializeField]
    protected IBattleEventTriggerUnit.ColliderDetectType detectType;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    protected bool useFlip;

    [SerializeField]
    protected float detectStartTime;

    [SerializeField]
    protected float detectDuration;

    [SerializeField]
    protected float followTime = 0;

    protected Dictionary<CharacterUnitModel, float> nextAllowedTime;

    private void Awake()
    {
        hitCollider.enabled = false;
        HideRenderer();
    }

    protected virtual async UniTask EnableColliderAsync()
    {
        if (detectStartTime > 0)
            await UniTaskUtils.DelaySeconds(detectStartTime, TokenPool.Get(GetHashCode()));

        hitCollider.enabled = true;

        if (detectDuration == 0)
        {
            await UniTask.NextFrame();
        }
        else
        {
            await UniTaskUtils.DelaySeconds(detectDuration, TokenPool.Get(GetHashCode()));
        }

        hitCollider.enabled = false;
    }

    public override async UniTask ShowAsync()
    {
        if (useFlip)
            Flip(Model.IsFlip);

        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            ObserverManager.AddObserver(BattleObserverID.EnemyKilled, this);

            if (nextAllowedTime == null)
                nextAllowedTime = new Dictionary<CharacterUnitModel, float>();
        }

        var offset = useFlip ? GetFlipLocalPos(Model.IsFlip) : LocalPosOffset;

        if (Model.FollowTarget != null)
        {
            FollowAsync(offset).Forget();
        }
        else
        {
            transform.localPosition += offset;
        }

        ShowRenderer();

        await EnableColliderAsync();
        await base.ShowAsync();
    }

    protected async UniTask FollowAsync(Vector3 localPosOffset)
    {
        float startTime = Time.time;
        
        while (!gameObject.CheckSafeNull() && gameObject.activeSelf)
        {
            if (followTime > 0 && Time.time - startTime >= followTime)
                break;
                
            transform.position = Model.FollowTarget.transform.position;
            transform.localPosition += localPosOffset;
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Enter)
            return;

        OnDetectHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Stay)
            return;

        OnDetectHitWithCooltime(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Exit)
            return;

        OnDetectHit(other);
    }

    protected virtual void OnDetectHitWithCooltime(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character) || Model == null)
            return;

        var targetModel = BattleSceneManager.Instance.GetCharacterModel(other);

        if (targetModel == null)
            return;

        float now = Time.time;
        float cd = FloatDefine.COLLIDER_STAY_COOLTIME_PER_TARGET;

        if (nextAllowedTime.TryGetValue(targetModel, out var t) && now < t)
            return;

        Model.OnEventHit(other, transform.position);
        nextAllowedTime[targetModel] = now + cd;
    }

    protected virtual void OnDetectHit(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        if (Model == null)
            return;

        Model.OnEventHit(other, transform.position);
    }

    protected override void OnDisable()
    {
        CleanUp();

        base.OnDisable();
    }

    protected void CleanUp()
    {
        TokenPool.Cancel(GetHashCode());
        hitCollider.enabled = false;

        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            ObserverManager.RemoveObserver(BattleObserverID.EnemyKilled, this);

            if (nextAllowedTime != null)
                nextAllowedTime.Clear();
        }
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam param)
            return;

        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            if (nextAllowedTime.ContainsKey(param.ModelValue))
                nextAllowedTime.Remove(param.ModelValue);
        }
    }
}
