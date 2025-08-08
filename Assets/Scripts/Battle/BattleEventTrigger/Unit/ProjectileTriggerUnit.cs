using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileTriggerUnit : PoolableBaseUnit<ProjectileTriggerUnitModel>, IBattleEventTriggerUnit
{
    public DirectionType StartDirectionType => startDirectionType;

    [SerializeField]
    private DirectionType startDirectionType = DirectionType.Owner;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    private float launchDelay = 0f;

    [SerializeField]
    private float fadeTime = 0.25f;

    protected Vector3 startPosition;
    protected Vector2 direction;

    protected bool acitvate;
    protected int hitCount;

    private void Awake()
    {
        hitCollider.enabled = false;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (effectSprite == null)
            effectSprite = GetComponent<SpriteRenderer>();
#endif
    }

    public override async UniTask ShowAsync()
    {
        if (Model == null)
            return;

        effectSprite.enabled = false;

        if (launchDelay > 0)
            await UniTaskUtils.DelaySeconds(launchDelay, TokenPool.Get(GetHashCode()));

        Launch();

        effectSprite.enabled = true;
    }


    protected override void OnDisable()
    {
        TokenPool.Cancel(GetHashCode());
        hitCollider.enabled = false;
        acitvate = false;

        base.OnDisable();
    }

    protected virtual void Launch()
    {
        direction = Model.StartDirection;
        hitCount = 0;
        bool isFlip = direction.x < 0;
        
        Vector3 worldPosition = Model.StartPosition;
        Vector3 offset = isFlip ? GetFlipLocalPos(isFlip) : LocalPosOffset;
        transform.position = worldPosition + offset;
        
        startPosition = transform.position;
        effectSprite.flipX = isFlip;
        hitCollider.enabled = true;
        effectSprite.RestoreAlpha();
        gameObject.SafeSetActive(true);

        acitvate = true;
    }

    private void FixedUpdate()
    {
        if (acitvate == false)
            return;

        UpdateMove();
        CheckDisable();
    }

    protected virtual void UpdateMove()
    {
        transform.position += Model.Speed * Time.fixedDeltaTime * (Vector3)direction;
    }

    private void CheckDisable()
    {
        if (CheckDisableCondition())
            Deactivate();
    }

    private bool CheckDisableCondition()
    {
        if (Model.Distance <= Vector3.Distance(transform.position, startPosition))
            return true;

        if (!acitvate)
            return true;

        if (Model.HitCount > 0 && hitCount >= Model.HitCount)
            return true;

        return false;
    }

    private void RotateSpriteToDirection()
    {
        if (direction == Vector2.zero)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        effectSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!acitvate)
            return;

        if (!other.gameObject.CheckLayer(LayerInt.Character))
            return;

        Model.OnEventHit(other);
    }

    public override void Deactivate()
    {
        acitvate = false;
        effectSprite.DeactiveWithFade(fadeTime, gameObject);
    }
}