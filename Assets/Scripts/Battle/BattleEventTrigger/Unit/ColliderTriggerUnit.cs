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
        AddEnemyKilledObserver();

        if (useFlip)
            Flip(Model.IsFlip);

        var offset = useFlip ? GetFlipLocalPos(Model.IsFlip) : LocalPosOffset;

        if (Model.IsEnableFollow)
        {
            FollowAsync(offset, followTime).Forget();
        }
        else
        {
            transform.localPosition += offset;
        }

        ShowRenderer();

        await EnableColliderAsync(detectStartTime, detectDuration);
    }
}