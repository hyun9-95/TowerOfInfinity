using Cysharp.Threading.Tasks;
using UnityEngine;


public class ColliderEnterTriggerUnit : PoolableBaseUnit<RangeTriggerUnitModel>, IBattleEventTriggerUnit
{
    protected enum ColliderSizeType
    {
        Fixed,
        Dynamic,
    }

    [SerializeField]
    protected ColliderSizeType colliderSizeType;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    protected bool useFlip;

    [SerializeField]
    protected float detectTime;

    private Vector2 originSize;
    private float originRadius;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private bool followTarget = false;

    private void Awake()
    {
        hitCollider.enabled = false;

        if (colliderSizeType == ColliderSizeType.Dynamic)
        {
            if (hitCollider is CircleCollider2D circleColliderTemp)
            {
                circleCollider = circleColliderTemp;
                circleCollider.radius = originRadius;
            }

            if (hitCollider is BoxCollider2D boxColliderTemp)
            {
                boxCollider = boxColliderTemp;
                originSize = boxCollider.size;
            }
        }

        HideRenderer();
    }

    protected void EnableCollider()
    {
        if (colliderSizeType == ColliderSizeType.Dynamic)
        {
            if (circleCollider != null)
            {
                circleCollider.radius = originRadius * Model.Range;
            }
            else if (boxCollider != null)
            {
                boxCollider.size = new Vector2(originSize.x * Model.Range, originSize.y * Model.Range);
            }
        }
        
        hitCollider.enabled = true;
    }

    public override async UniTask ShowAsync()
    {
        if (useFlip)
            Flip(Model.IsFlip);

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

        if (detectTime > 0)
            await UniTaskUtils.DelaySeconds(detectTime, TokenPool.Get(GetHashCode()));

        EnableCollider();

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

    protected virtual void OnTriggerEnter2D(Collider2D other)
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
