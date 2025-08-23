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
        acitvated = false;
        gameObject.SetActive(false);
    }

    // 카메라 팔로우 댐핑때문에 LateUpdate하면 간극이 커져서 HpBar는 Fixed에서 처리
    private void FixedUpdate()
    {
        if (!acitvated)
            return;

        if (Model.Owner.IsDead)
        {
            Hide();
            return;
        }

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
