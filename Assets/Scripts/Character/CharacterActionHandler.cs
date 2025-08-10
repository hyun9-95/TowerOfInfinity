using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CharacterActionHandler
{
    public bool IsRolling => rolling;
    public bool IsAddingForce => addingForce;

    private Action<bool> onFlipX;

    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer bodySprite;
    private GameObject gameObject;
    private IPathFinder pathFinder;
    private CharacterAnimationEffect animEffect;

    #region Hit
    private bool blinking = false;
    private bool rolling = false;
    private bool addingForce = false;
    private Color originColor;
    #endregion

    public CharacterActionHandler(Animator animator, Rigidbody2D rigidBody2D, SpriteRenderer bodySprite, GameObject gameObject, IPathFinder pathFinder)
    {
        this.animator = animator;
        this.rigidBody2D = rigidBody2D;
        this.bodySprite = bodySprite;
        this.gameObject = gameObject;
        this.pathFinder = pathFinder;

        originColor = bodySprite.color;
    }

    public void SetCharacterAnimEffect(CharacterAnimationEffect animEffect)
    {
        this.animEffect = animEffect;
    }

    public void SetOnFlipX(Action<bool> action)
    {
        onFlipX = action;
    }

    public void OnMovement(Vector2 movement, float speed, bool enableFlip)
    {
        if (rigidBody2D == null)
            return;

        rigidBody2D.MovePosition(rigidBody2D.position + speed * Time.fixedDeltaTime * movement.normalized);

        if (enableFlip)
            Flip(movement);
    }

    public async UniTask OnAddForceAsync(Vector2 direction, float force)
    {
        if (rigidBody2D == null)
            return;

        addingForce = true;

        Vector2 velocity = direction.normalized * force;

        float elapsedTime = 0;
        
        while (gameObject != null && gameObject.activeSelf && elapsedTime < FloatDefine.ADD_FORCE_DURATION)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            elapsedTime += Time.fixedDeltaTime;

            rigidBody2D.MovePosition(rigidBody2D.position + (velocity * Time.fixedDeltaTime));
            velocity *= FloatDefine.ADD_FORCE_DAMPING;
        }

        addingForce = false;
    }

    public void OnNavmeshPathFind(Vector3 targetPos)
    {
        if (pathFinder is NavmeshPathFinder navmeshPathFinder)
        {
            var dir = navmeshPathFinder.OnPathFindDirection(targetPos);
            Flip(dir);
        }
    }

    public void OnAStarUpdatePath(Vector3 targetPos)
    {
        if (pathFinder is AStarPathFinder astarPathFinder)
            astarPathFinder.OnPathFind(targetPos);
    }

    /// <summary>
    /// 계산된 경로 따라 이동
    /// </summary>
    public void OnAStarMoveAlongPath()
    {
        if (addingForce)
            return;
            
        if (pathFinder is AStarPathFinder astarFinder)
        {
            var dir = astarFinder.OnMoveAlongPath();
            Flip(dir);
        }
    }

    public void OnStopPathFind()
    {
        pathFinder.OnStopPathFind();
    }

    public async UniTask OnHitEffectAsync(Color hitColor)
    {
        if (blinking)
            return;

        blinking = true;

        int blinkCount = 0;

        while (blinkCount < IntDefine.HIT_BLINK_COUNT && blinking)
        {
            bodySprite.color = hitColor;
            await UniTaskUtils.DelaySeconds(FloatDefine.HIT_BLINK_INTERVAL, TokenPool.Get(GetHashCode()));

            bodySprite.color = originColor;
            await UniTaskUtils.DelaySeconds(FloatDefine.HIT_BLINK_INTERVAL / 2, TokenPool.Get(GetHashCode()));

            blinkCount++;
        }

        bodySprite.color = originColor;
        blinking = false;
    }

    public async UniTask OnRollingAsync(float rollDelay, Vector2 direction, float force)
    {
        if (rolling)
            return;

        rolling = true;
        await UniTaskUtils.DelaySeconds(rollDelay, cancellationToken: TokenPool.Get(GetHashCode()));

        if (animEffect != null)
            animEffect.Play(CharacterAnimationEffect.EffectType.Dash, rigidBody2D.position, direction.x < 0);

        await OnAddForceAsync(direction, force);
        rolling = false;
    }

    private void Flip(Vector2 movement)
    {
        bodySprite.flipX = movement.x < 0;

        onFlipX?.Invoke(bodySprite.flipX);
    }

    public void Cancel()
    {
        TokenPool.Cancel(GetHashCode());
        blinking = false;
        rolling = false;
        addingForce = false;
    }
}
