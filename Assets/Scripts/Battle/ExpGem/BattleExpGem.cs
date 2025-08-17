#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class BattleExpGem : PoolableBaseUnit<BattleExpGemModel>
{
    [SerializeField]
    private AnimationCurve accelerationCurve;

    [SerializeField]
    private float accelerationDuration = 1f;

    [SerializeField]
    private float spawnBouncePower = 8f;

    [SerializeField]
    private float bounceDuration = 0.2f;

    [SerializeField]
    private float startMoveSpeed = 5f;

    [SerializeField]
    private float maxMoveSpeed = 15f;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Collider2D gemCollider;

    private void OnEnable()
    {
        gemCollider.enabled = true;
    }

    public async UniTask BounceAsync(Vector2 killerDirection)
    {
        gameObject.SafeSetActive(true);

        try
        {
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;
            var token = TokenPool.Get(GetHashCode());
            
            // 수평으로만 이동, Y는 시작점과 동일
            Vector3 targetPosition = startPosition + new Vector3(killerDirection.x * spawnBouncePower, 0f, 0f);
            
            while (!token.IsCancellationRequested && elapsedTime < bounceDuration)
            {
                float progress = elapsedTime / bounceDuration;
                
                // 수평 값 보간
                float currentX = Mathf.Lerp(startPosition.x, targetPosition.x, progress);
                
                // PI를 곱해야 위로만 튕김
                float height = spawnBouncePower * Mathf.Sin(progress * Mathf.PI);
                
                transform.position = new Vector3(currentX, startPosition.y + height, startPosition.z);

                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async UniTask MoveToTarget(Transform target, Action onArrive)
    {
        TokenPool.Cancel(GetHashCode());

        gemCollider.enabled = false;

        float elapsedTime = 0f;

        Vector3 initialDir = (target.position - transform.position).normalized;
        Vector3 bounceDir = initialDir * -1;

        while (Vector2.Distance(target.position, transform.position) > BattleDefine.EXP_GEM_DISTANCE_THRESHOLD)
        {
            float accelerateTime = Mathf.Clamp01(elapsedTime / accelerationDuration);
            float curveValue = accelerationCurve.Evaluate(accelerateTime);
            float speed = Mathf.Lerp(startMoveSpeed,
                                     maxMoveSpeed,
                                     curveValue);

            Vector3 dir;
            Vector3 targetPos = target.position + offset;
            if (elapsedTime < bounceDuration)
            {
                // 바운스: 뒤로 이동하지만 점차 타겟 방향으로 전환
                float bounceProgress = elapsedTime / bounceDuration;
                Vector3 targetDir = (targetPos - transform.position).normalized;
                dir = Vector3.Lerp(bounceDir, targetDir, bounceProgress);
            }
            else
            {
                dir = (targetPos - transform.position).normalized;
            }

            transform.position += speed * Time.deltaTime * dir;

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
        }

        onArrive?.Invoke();
        gameObject.SafeSetActive(false);
    }
}
