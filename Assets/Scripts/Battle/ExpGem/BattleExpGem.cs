#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class BattleExpGem : PoolableBaseUnit<BattleExpGemModel>
{
    [SerializeField]
    private AnimationCurve accelerationCurve;

    [SerializeField]
    private float accelerationDuration = 1f;

    [SerializeField]
    private float bouncePower = 2f;

    [SerializeField]
    private float backwardPushPower = 0.5f;

    [SerializeField]
    private float bounceDuration = 0.3f;

    [SerializeField]
    private float startMoveSpeed = 3f;

    [SerializeField]
    private float maxMoveSpeed = 15f;

    public async UniTask MoveToTarget(Transform target, Action onArrive)
    {
        if (target == null)
            return;

        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        
        // 초기 타겟 방향
        Vector3 initialDir = (target.position - startPos).normalized;
        
        // 바운스 방향 : 옆으로 휘면서 튕기도록!
        
        // 1단계: 90도 회전한 벡터 생성 (수직 방향)
        // (x, y) → (-y, x) 변환으로 시계 반대방향 90도 회전
        Vector3 bounceDir = new Vector3(-initialDir.y, initialDir.x, 0f);
        
        // 2단계: 랜덤 방향 결정
        // -1 일 경우 위에서 구한 방향의 반대 방향.
        float randomDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f;
        bounceDir *= randomDirection;
        
        // 3단계: 뒤로 밀려나는 효과 추가
        // 타겟 반대 방향(-initialDir)으로 살짝 밀리는 느낌 주기
        Vector3 backwardPush = -initialDir * backwardPushPower;
        
        // 최종 바운스 방향: 랜덤 수직 방향 + 뒤로 밀리는 방향
        bounceDir += backwardPush;
        bounceDir = bounceDir.normalized; 

        // 1단계: 바운스 구간
        while (target != null && elapsedTime < bounceDuration)
        {
            float bounceTime = Mathf.Clamp01(elapsedTime / bounceDuration);
            float bounceMultiplier = Mathf.Lerp(bouncePower, 0f, bounceTime);
            float bounceOffset = Mathf.Sin(bounceTime * Mathf.PI) * bounceMultiplier;
            
            // 바운스 효과
            transform.position += bounceOffset * Time.deltaTime * bounceDir;

            elapsedTime += Time.deltaTime;
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }

        // 2단계: 타겟 추적 가속 구간
        while (target != null && Vector2.Distance(target.position, transform.position) > 0.5f)
        {
            // 가속 시간은 바운스 구간 이후부터 계산
            float accelerateTime = Mathf.Clamp01((elapsedTime - bounceDuration) / accelerationDuration);
            float curveValue = accelerationCurve.Evaluate(accelerateTime);
            float speed = Mathf.Lerp(startMoveSpeed, maxMoveSpeed, curveValue);

            // 타겟 방향으로 이동
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += speed * Time.deltaTime * dir;

            elapsedTime += Time.deltaTime;
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
        }

        onArrive?.Invoke();
        gameObject.SafeSetActive(false);
    }
}
