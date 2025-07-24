using Cysharp.Threading.Tasks;
using UnityEngine;


public class ColliderEnterTriggerUnit : PoolableBaseUnit<BattleEventTriggerUnitModel>, IBattleEventTriggerUnit
{
    protected enum ColliderSizeType
    {
        Fixed,
        Dynamic,
    }

    protected enum ColliderDetectType
    {
        Enter,
        Stay,
        Exit,
    }

    [SerializeField]
    protected ColliderDetectType detectType;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    protected bool useFlip;

    [SerializeField]
    protected bool useTargetFollow;

    [SerializeField]
    protected float detectStartTime;

    [SerializeField]
    protected float detectEndTime;


    private void Awake()
    {
        hitCollider.enabled = false;
        HideRenderer();
    }

    protected async UniTask EnableColliderAsync()
    {
        if (detectStartTime > 0)
            await UniTaskUtils.DelaySeconds(detectStartTime, TokenPool.Get(GetHashCode()));

        hitCollider.enabled = true;

        if (detectEndTime > 0)
        {
            await UniTaskUtils.DelaySeconds(detectEndTime, TokenPool.Get(GetHashCode()));
        }
        else
        {
            await UniTask.NextFrame();
        }

        hitCollider.enabled = false;
    }

    public override async UniTask ShowAsync()
    {
        if (useFlip)
            Flip(Model.IsFlip);

        var offset = useFlip ? GetFlipLocalPos(Model.IsFlip) : LocalPosOffset;

        if (Model.FollowTarget != null && useTargetFollow)
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
        while (!gameObject.CheckSafeNull() && gameObject.activeSelf)
        {
            transform.position = Model.FollowTarget.transform.position;
            transform.localPosition += localPosOffset;
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (detectType != ColliderDetectType.Enter)
            return;

        OnDetectHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (detectType != ColliderDetectType.Stay)
            return;

        OnDetectHit(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectType != ColliderDetectType.Exit)
            return;

        OnDetectHit(other);
    }

    protected virtual void OnDetectHit(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        if (Model == null)
            return;

        Model.OnEventHit(other);
    }

    protected override void OnDisable()
    {
        hitCollider.enabled = false;

        base.OnDisable();
    }
}
