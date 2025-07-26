using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CharacterStateActionHandler
{
    public IPathFinder PathFinder => pathFinder;

    private Action<bool> onFlipX;

    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer bodySprite;
    private GameObject gameObject;
    private IPathFinder pathFinder;

    #region Hit
    private bool blinking = false;
    private Color originColor;
    #endregion

    public CharacterStateActionHandler(Animator animator, Rigidbody2D rigidBody2D, SpriteRenderer bodySprite, GameObject gameObject, IPathFinder pathFinder)
    {
        this.animator = animator;
        this.rigidBody2D = rigidBody2D;
        this.bodySprite = bodySprite;
        this.gameObject = gameObject;
        this.pathFinder = pathFinder;

        originColor = bodySprite.color;
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

    public void OnAddForce(Vector2 direction, float force)
    {
        rigidBody2D.AddForce(direction * force, ForceMode2D.Impulse);
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

        while (blinkCount < IntDefine.HitBlinkCount && blinking)
        {
            bodySprite.color = hitColor;
            await UniTaskUtils.DelaySeconds(FloatDefine.HitBlinkInterval, TokenPool.Get(GetHashCode()));

            bodySprite.color = originColor;
            await UniTaskUtils.DelaySeconds(FloatDefine.HitBlinkInterval / 2, TokenPool.Get(GetHashCode()));

            blinkCount++;
        }

        bodySprite.color = originColor;
        blinking = false;
    }

    private void Flip(Vector2 movement)
    {
        bodySprite.flipX = movement.x < 0;

        onFlipX?.Invoke(bodySprite.flipX);
    }

    public void Cancel()
    {
        TokenPool.Cancel(GetHashCode());
    }
}
