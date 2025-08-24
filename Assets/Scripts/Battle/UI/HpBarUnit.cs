#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10000)]
public class HpBarUnit : BaseUnit<HpBarUnitModel>
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private Image hpBar;

    [SerializeField]
    private Vector2 localOffset;

    [SerializeField]
    private PlayerLoopTiming timing = PlayerLoopTiming.Initialization;

    private bool acitvated = false;
    private Camera worldCamera;

    public override async UniTask ShowAsync()
    {
        worldCamera = CameraManager.Instance.GetCinemachineBrain().OutputCamera;
        acitvated = true;
        gameObject.SetActive(true);

        UpdatePos();
        UpdatePosAsync().Forget();
    }

    public void Hide()
    {
        acitvated = false;
        gameObject.SetActive(false);
    }

    private async UniTask UpdatePosAsync()
    {
        while (acitvated && gameObject.SafeActiveSelf())
        {
            UpdatePos();
            await UniTask.Yield(timing);
        }
    }

    private void UpdatePos()
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
            worldCamera,
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
