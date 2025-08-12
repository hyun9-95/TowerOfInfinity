using UnityEngine;

public class BouncingProjectileTriggerUnit : ProjectileTriggerUnit
{
    private float lastBounceTime = 0f;
    private float bounceCooldown = 0.1f;

    protected override void OnDetectHit(Collider2D other)
    {
        if (!moveUpdate)
            return;

        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        // 적과 충돌 시 바운스
        var targetModel = BattleSceneManager.Instance.GetCharacterModel(other);

        if (targetModel == null || targetModel.TeamTag != Model.DetectTeamTag)
            return;

        if (Time.time - lastBounceTime < bounceCooldown)
            return;

        lastBounceTime = Time.time;
        BounceOffTarget(other);
        Model.OnEventHit(other, transform.position);
    }

    private void BounceOffTarget(Collider2D hitTarget)
    {
        // 충돌면의 법선 벡터 (적에서 투사체로 향하는 방향)
        Vector2 normal = (transform.position - hitTarget.transform.position).normalized;
        
        // 벡터 반사 공식: 반사벡터 = 입사벡터 - 2 × (입사벡터·법선벡터) × 법선벡터
        // R = D - 2(D·N)N
        // R: 반사된 방향 (Reflected direction)
        // D: 현재 진행 방향 (Direction)  
        // N: 충돌면의 법선 벡터 (Normal)
        Vector2 currentDirection = direction.normalized;  // D
        Vector2 reflectedDirection = currentDirection - 2f * Vector2.Dot(currentDirection, normal) * normal;  // R
        
        direction = reflectedDirection.normalized;
    }
}