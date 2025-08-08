using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileTriggerUnit : PoolableBaseUnit<ProjectileTriggerUnitModel>, IBattleEventTriggerUnit
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
    private Vector2 direction;

    private bool acitvate;

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
            await UniTaskUtils.DelaySeconds(launchDelay);

        Launch();

        effectSprite.enabled = true;
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        hitCollider.enabled = false;
        acitvate = false;
    }

    private void Launch()
    {
        direction = Model.StartDirection;
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

    private void UpdateMove()
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
        if (!acitvate)
            return;

        Model.OnEventHit(other);
    }

    public void Deactivate()
    {
        acitvate = false;
        effectSprite.DeactiveWithFade(fadeTime, gameObject);
    }
}