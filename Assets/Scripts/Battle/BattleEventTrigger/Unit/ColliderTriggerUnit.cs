using Cysharp.Threading.Tasks;
using UnityEngine;

public class ColliderTriggerUnit : BaseTriggerUnit<BattleEventTriggerUnitModel>
{
    [SerializeField]
    protected bool useFlip;

    [SerializeField]
    protected float detectStartTime;

    [SerializeField]
    protected float detectDuration;

    [SerializeField]
    protected float followTime = 0;

    #region Lifecycle
    protected override void OnUnitAwake()
    {
        HideRenderer();
    }

    protected override void OnUnitDisable()
    {
        base.OnUnitDisable();
    }
    #endregion

    public override async UniTask ShowAsync()
    {
        if (useFlip)
            Flip(Model.IsFlip);

        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            InitializeStayCooldown();
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
}