using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileUnit : PoolableBaseUnit<ProjectileUnitModel>
{
    public DirectionType DirectionType => directionType;

    [SerializeField]
    private DirectionType directionType = DirectionType.Owner;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    private float launchDelay = 0.2f;

    [SerializeField]
    private float fadeTime = 0.5f;

    private Vector3 startPosition;
    private Vector3 originScale;
    private Vector2 direction;

    private bool launched;

    private void Awake()
    {
        hitCollider.enabled = false;
        originScale = transform.localScale;
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

        await UniTaskUtils.DelaySeconds(launchDelay);

        Launch();

        effectSprite.enabled = true;
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        hitCollider.enabled = false;
        launched = false;
    }

    private void Launch()
    {
        launched = true;
        bool isFlip = direction.x < 0;
        transform.position = Model.StartPosition;
        transform.localPosition += isFlip ? GetFlipLocalPos(isFlip) : LocalPosOffset;
        startPosition = transform.position;
        direction = Model.StartDirection;
        effectSprite.flipX = isFlip;
        hitCollider.enabled = true;
        effectSprite.RestoreAlpha();
        transform.localScale = originScale * Model.Scale;
        gameObject.SafeSetActive(true);
    }

    private void FixedUpdate()
    {
        if (launched == false)
            return;

        UpdateDirection();
        UpdateMove();
        CheckDisable();
    }

    protected void UpdateDirection()
    {
        if (!directionType.IsFixedType())
            direction = Model.OnUpdateDirection(directionType);
    }

    private void UpdateMove()
    {
        transform.Translate(Model.Speed * Time.fixedDeltaTime * direction.normalized);
    }

    private void CheckDisable()
    {
        if (IsDisable())
        {
            launched = false;
            effectSprite.FadeOff(fadeTime, gameObject);
        }
    }

    private bool IsDisable()
    {
        if (Model.Distance <= Vector3.Distance(transform.position, startPosition))
            return true;

        if (Model.IsOverTargetCount())
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Model.OnEventHit(other);
    }
}