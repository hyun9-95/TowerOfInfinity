using Cysharp.Threading.Tasks;
using System;
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

    protected async UniTask CheckLifeTime(float lifeTime)
    {
        isCheckingLifeTime = true;

        await UniTaskUtils.DelaySeconds(lifeTime, cancellationToken: TokenPool.Get(GetHashCode()));
        
        if (gameObject)
            gameObject.SafeSetActive(false);

        isCheckingLifeTime = false;
    }

    protected void Deactivate(float fadeTime = 0)
    {
        if (fadeTime > 0 && effectSprite != null)
        {
            effectSprite.DeactiveWithFade(fadeTime, gameObject);
        }
        else
        {
            gameObject.SafeSetActive(false);
        }
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
                transform.localScale = new Vector3(value ? originLocalScale.x * -1 : originLocalScale.x
                    , originLocalScale.y, originLocalScale.z);
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
            effectSprite.RestoreAlpha();
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
            CheckLifeTime(lifeTime).Forget();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isCheckingLifeTime = false;
    }
}