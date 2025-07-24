using Cysharp.Threading.Tasks;
using UnityEngine;

public class HitTargetRangeUnit : PoolableBaseUnit<RangeTriggerUnitModel>
{
    [SerializeField]
    private float detectTime = 0f;

    public override async UniTask ShowAsync()
    {
        if (detectTime > 0)
            await UniTaskUtils.DelaySeconds(detectTime);

        var colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, Model.Range, (int)LayerFlag.Character);

        if (colliders == null || colliders.Length == 0)
            return;

        colliders.SortByNearest(gameObject.transform.position);

        foreach (var collider in colliders)
            Model.OnEventHit(collider);
    }

    private void OnDrawGizmos()
    {
        if (Model != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Model.Range);
        }
    }
}
