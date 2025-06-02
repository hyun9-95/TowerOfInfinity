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

    public async UniTask MoveToTarget(Vector3 targetPos, Action onArrive)
    {
        float elapsedTime = 0f;

        while (Vector2.Distance(targetPos, transform.position) > 0.5f)
        {
            float t = Mathf.Clamp01(elapsedTime / accelerationDuration);
            float curveValue = accelerationCurve.Evaluate(t);
            float speed = Mathf.Lerp(FloatDefine.DEFAULT_BATTLE_EXP_GEM_MOVE_SPEED,
                                     FloatDefine.DEFAULT_BATTLE_EXP_GEM_MOVE_MAX_SPEED,
                                     curveValue);

            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += speed * Time.fixedDeltaTime * dir;

            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate();
        }

        onArrive?.Invoke();
        gameObject.SafeSetActive(false);
    }
}
