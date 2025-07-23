#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUnit : BaseUnit<HpBarUnitModel>
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private Image hpBar;

    [SerializeField]
    private Vector2 localOffset;

    private bool acitvated = false;

    public override async UniTask ShowAsync()
    {
        acitvated = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!acitvated)
            return;

        if (Model == null || Model.Owner == null)
            return;

        hpBar.fillAmount = Model.GetHpBarValue();

        rectTransform.FollowWorldPosition(
            Model.Owner.Transform.position,
            Model.BattleUICamera,
            Model.BattleUICanvasRect,
            localOffset
        );
    }

    private void OnDisable()
    {
        acitvated = false;
    }
}
