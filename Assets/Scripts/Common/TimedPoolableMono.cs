#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimedPoolableMono : PoolableMono
{
    public Vector3 LocalPosOffset => localPosOffset;

    [SerializeField]
    protected float lifeTime;

    [SerializeField]
    protected float fadeTime = 0f;

    [SerializeField]
    protected float fadeInTime = 0f;

    [SerializeField]
    protected float awaitTime = 0f;

    [SerializeField]
    protected Vector3 localPosOffset;

    [SerializeField]
    protected SpriteRenderer effectSprite;

    [SerializeField]
    protected Animator spriteAnimator;

    [SerializeField]
    protected ParticleSystem effectParticle;

    [SerializeField]
    protected ParticleSystemRenderer particleSystemRenderer;

    private bool isCheckingLifeTime = false;

    protected Vector3 originLocalScale;

    private void Awake()
    {
        originLocalScale = transform.localScale;
    }

    public virtual void Refresh() { }

    public virtual async UniTask ShowAsync()
    {
        ShowRenderer();
        gameObject.SafeSetActive(true);
    }

    public async UniTask ShowAwaitLifeTime()
    {
        ShowAsync().Forget();

        if (lifeTime > 0 && awaitTime > 0)
            await UniTaskUtils.DelaySeconds(awaitTime, cancellationToken: TokenPool.Get(GetHashCode()));
    }

    protected async UniTask CheckLifeTime()
    {
        isCheckingLifeTime = true;

        await UniTaskUtils.DelaySeconds(lifeTime, cancellationToken: TokenPool.Get(GetHashCode()));
        
        if (gameObject)
            Deactivate();

        isCheckingLifeTime = false;
    }

    protected void Deactivate()
    {
        if (fadeTime > 0 && effectSprite != null)
        {
            if (spriteAnimator != null)
                spriteAnimator.speed = 0;

            effectSprite.DeactiveWithFade(fadeTime, gameObject);
        }
        else
        {
            gameObject.SafeSetActive(false);
        }
    }

    protected void FadeIn(float fadeInTime)
    {
        if (effectSprite != null)
            effectSprite.FadeIn(fadeInTime);
    }

    public void Flip(bool value)
    {
        if (effectSprite)
        {
            effectSprite.flipX = value;
        }
        else
        {
            if (particleSystemRenderer != null)
            {
                float flipX = value ? 1f : 0f;
                particleSystemRenderer.flip = new Vector3(flipX, 0, 0);
            }
            else
            {
                float rot = value ? 180 : 0;

                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }

    public void HideRenderer()
    {
        if (effectSprite != null)
            effectSprite.enabled = false;

        if (effectParticle != null)
            effectParticle.Stop();
    }

    public void ShowRenderer()
    {
        if (effectSprite != null)
        {
            effectSprite.enabled = true;
            
            if (spriteAnimator != null)
            {
                spriteAnimator.speed = 1f;
                spriteAnimator.ResetCurrentState();
            }

            if (fadeInTime > 0)
            {
                effectSprite.FadeIn(fadeInTime);
            }
            else
            {
                effectSprite.RestoreAlpha();
            }   
        }

        if (effectParticle != null)
            effectParticle.Play();
    }

    protected Vector3 GetFlipLocalPos(bool isFlip)
    {
        return new Vector3(isFlip ? -localPosOffset.x : localPosOffset.x, localPosOffset.y, localPosOffset.z);
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