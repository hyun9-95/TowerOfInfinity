using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimedPoolableMono : PoolableMono
{
    public Vector3 LocalPosOffset => localPosOffset;

    [SerializeField]
    protected float lifeTime;

    [SerializeField]
    protected Vector3 localPosOffset;

    [SerializeField]
    protected SpriteRenderer effectSprite;

    private bool isCheckingLifeTime = false;

    protected Vector3 originLocalScale;

    private void Awake()
    {
        originLocalScale = transform.localScale;
    }

    private async UniTask CheckLifeTime()
    {
        isCheckingLifeTime = true;

        await UniTaskUtils.DelaySeconds(lifeTime, cancellationToken: TokenPool.Get(GetHashCode()));
        gameObject.SafeSetActive(false);

        isCheckingLifeTime = false;
    }

    public void Flip(bool value)
    {
        if (effectSprite)
        {
            effectSprite.flipX = value;
        }
        else
        {
            transform.localScale = new Vector3(value ? originLocalScale.x * -1 : originLocalScale.x
                , originLocalScale.y, originLocalScale.z);
        }
    }

    private void OnEnable()
    {
        if (lifeTime == 0)
            return;

        if (!isCheckingLifeTime)
            CheckLifeTime().Forget();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isCheckingLifeTime = false;
    }
}