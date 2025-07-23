using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CharacterStateActionHandler
{
    public IPathFinder PathFinder => pathFinder;

    private Action onDeactivate;
    private Action<bool> onFlipX;

    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer bodySprite;
    private GameObject gameObject;
    private IPathFinder pathFinder;

    public CharacterStateActionHandler(Animator animator, Rigidbody2D rigidBody2D, SpriteRenderer bodySprite, GameObject gameObject, IPathFinder pathFinder)
    {
        this.animator = animator;
        this.rigidBody2D = rigidBody2D;
        this.bodySprite = bodySprite;
        this.gameObject = gameObject;
        this.pathFinder = pathFinder;
    }

    public void SetOnDeactivate(Action action)
    {
        onDeactivate = action;
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

    public void OnDead()
    {
        DeadAsync().Forget();
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

    private void Flip(Vector2 movement)
    {
        bodySprite.flipX = movement.x < 0;

        onFlipX?.Invoke(bodySprite.flipX);
    }

    private async UniTask DeadAsync()
    {
        onDeactivate?.Invoke();

        var targetModel = BattleSceneManager.Instance.GetCharacterModel(gameObject.GetInstanceID());
        var expGemModel = targetModel != null && targetModel.TeamTag == TeamTag.Enemy ? new BattleExpGemModel() : null;

        if (BattleSceneManager.Instance != null)
            BattleSceneManager.Instance.RemoveLiveCharacter(gameObject.GetInstanceID());

        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        bodySprite.FadeOff(1, gameObject);

        if (expGemModel != null)
        {
            var exp = await ObjectPoolManager.Instance.SpawnPoolableMono<BattleExpGem>(PathDefine.BATTLE_EXP_GEM, gameObject.transform.position, Quaternion.identity);
            exp.SetModel(expGemModel);
            exp.ShowAsync().Forget();
        }
    }
}
