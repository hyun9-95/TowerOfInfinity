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
    private bool rotateToDirection = true;

    protected Vector3 startPosition;
    protected Vector2 direction;
    protected bool moveUpdate;

    private void FixedUpdate()
    {
        if (moveUpdate == false)
            return;

        UpdateMove();
        CheckDisable();
    }

    protected override void OnUnitDisable()
    {
        moveUpdate = false;
        base.OnUnitDisable();
    }

    public override async UniTask ShowAsync()
    {
        AddEnemyKilledObserver();

        if (Model == null)
            return;

        if (launchDelay > 0)
        {
            HideRenderer();
            await UniTaskUtils.DelaySeconds(launchDelay, TokenPool.Get(GetHashCode()));
        }

        Launch();
        ShowRenderer(fadeInTime);
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
        gameObject.SafeSetActive(true);

        moveUpdate = true;
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
            DeactiveWithStopMove();
    }

    private bool CheckDisableCondition()
    {
        if (Model.MoveDistance <= Vector3.Distance(transform.position, startPosition))
            return true;

        if (!moveUpdate)
            return true;

        if (Model.IsOverHitCount())
            return true;

        return false;
    }

    protected void DeactiveWithStopMove()
    {
        moveUpdate = false;
        DeactivateWithFade();
    }

    protected override void OnDetectHit(Collider2D other)
    {
        if (!moveUpdate)
            return;

        base.OnDetectHit(other);
    }
}