using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileTriggerUnit : BaseTriggerUnit<ProjectileTriggerUnitModel>
{
    public DirectionType StartDirectionType => startDirectionType;

    [SerializeField]
    private DirectionType startDirectionType = DirectionType.Owner;

    [SerializeField]
    private float launchDelay = 0f;

    [SerializeField]
    private float fadeTime = 0.25f;

    [SerializeField]
    private bool rotateToDirection = true;

    protected Vector3 startPosition;
    protected Vector2 direction;
    protected bool acitvate;

    private void FixedUpdate()
    {
        if (acitvate == false)
            return;

        UpdateMove();
        CheckDisable();
    }

    protected override void OnUnitDisable()
    {
        acitvate = false;
        base.OnUnitDisable();
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

    protected virtual void UpdateMove()
    {
        transform.position += Model.Speed * Time.fixedDeltaTime * (Vector3)direction;
        RotateSpriteToDirection();
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

    public override void Deactivate()
    {
        acitvate = false;
        effectSprite.DeactiveWithFade(fadeTime, gameObject);
    }

    protected override void OnDetectHit(Collider2D other)
    {
        if (!acitvate)
            return;

        base.OnDetectHit(other);
    }
}