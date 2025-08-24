using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackholeColliderTriggerUnit : BaseTriggerUnit<BlackholeTriggerUnitModel>
{
    [SerializeField]
    protected bool useFlip;

    [SerializeField]
    protected float detectStartTime;

    [SerializeField]
    protected float detectDuration;

    [SerializeField]
    protected float followTime = 0;

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