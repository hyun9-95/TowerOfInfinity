using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileTriggerUnit : PoolableBaseUnit<ProjectileTriggerUnitModel>, IBattleEventTriggerUnit
{
    public DirectionType StartDirectionType => startDirectionType;

    [SerializeField]
    protected IBattleEventTriggerUnit.ColliderDetectType detectType;

    [SerializeField]
    private DirectionType startDirectionType = DirectionType.Owner;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    private float launchDelay = 0f;

    [SerializeField]
    private float fadeTime = 0.25f;

    [SerializeField]
    private bool rotateToDirection = true;

    protected Vector3 startPosition;
    protected Vector2 direction;

    protected bool acitvate;

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
        SetStartDirection();
        bool isFlip = direction.x < 0;
        
        Vector3 worldPosition = Model.StartPosition;
        Vector3 offset = isFlip ? GetFlipLocalPos(isFlip) : LocalPosOffset;
        transform.position = worldPosition + offset;
        
        startPosition = transform.position;
        RotateSpriteToDirection();
        hitCollider.enabled = true;
        effectSprite.RestoreAlpha();
        gameObject.SafeSetActive(true);

        acitvate = true;
    }

    protected virtual void SetStartDirection()
    {
        direction = Model.StartDirection;
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
        RotateSpriteToDirection();
    }

    private void CheckDisable()
    {
        if (CheckDisableCondition())
            Deactivate();
    }

    private bool CheckDisableCondition()
    {
        if (Model.MoveDistance <= Vector3.Distance(transform.position, startPosition))
            return true;

        if (!acitvate)
            return true;

        if (Model.IsOverHitCount())
            return true;

        return false;
    }

    protected void RotateSpriteToDirection()
    {
        if (direction == Vector2.zero)
            return;

        if (!rotateToDirection)
        {
            effectSprite.flipX = direction.x < 0;
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        effectSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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

        OnDetectHit(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Exit)
            return;

        OnDetectHit(other);
    }

    protected virtual void OnDetectHit(Collider2D other)
    {
        if (!acitvate)
            return;

        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        if (Model == null)
            return;

        Model.OnEventHit(other);
    }

    public override void Deactivate()
    {
        acitvate = false;
        effectSprite.DeactiveWithFade(fadeTime, gameObject);
    }
}