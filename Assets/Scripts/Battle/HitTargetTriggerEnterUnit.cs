using Cysharp.Threading.Tasks;
using UnityEngine;

public class HitTargetTriggerEnterUnit : PoolableBaseUnit<HitTargetRangeUnitModel>
{
    [SerializeField]
    protected CircleCollider2D hitCollider;

    private void Awake()
    {
        hitCollider.enabled = false;
    }

    protected void EnableCollider()
    {
        hitCollider.radius = Model.Range;
        hitCollider.enabled = true;
    }

    public override async UniTask ShowAsync()
    {
        EnableCollider();

        await base.ShowAsync();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        if (Model == null)
            return;

        Model.OnEventHit(other);
    }

    protected override void OnDisable()
    {
        hitCollider.enabled = false;

        base.OnDisable();
    }
}
