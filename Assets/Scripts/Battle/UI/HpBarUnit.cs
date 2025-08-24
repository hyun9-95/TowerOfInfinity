#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
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
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdatePos);
    }

    public void Hide()
    {
        acitvated = false;
        gameObject.SetActive(false);
    }

    private void UpdatePos(CinemachineBrain brain)
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
            brain.OutputCamera,
            Model.BattleUICamera,
            Model.BattleUICanvasRect,
            localOffset
        );
    }

    private void OnDisable()
    {
        acitvated = false;
        CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdatePos);
    }
}
