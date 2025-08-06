using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleExpGainer : PoolableBaseUnit<BattleExpGainerModel>
{
    [SerializeField]
    private CircleCollider2D expCollider;

    private bool activate = false;

    private void Awake()
    {
        expCollider.enabled = false;
        activate = false;
    }

    private void LateUpdate()
    {
        if (!activate)
            return;

        if (Model.Owner == null || Model.Owner.Transform == null)
            return;

        transform.position = Model.Owner.Transform.position;
    }

    public void Activate(bool value)
    {
        expCollider.enabled = value;
        activate = value;
    }

    public void UpdateRadius()
    {
        expCollider.radius = Formula.GetExpGainRangeRadius(Model.Level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CheckLayer(LayerFlag.Object))
            return;

        if (!collision.gameObject.TryGetComponent<BattleExpGem>(out var gem))
            return;

        gem.MoveToTarget(transform, () =>
        {
            var exp = gem.Model.BattleExp;

            Model.OnExpGain(exp);
        }).Forget();
    }
}
