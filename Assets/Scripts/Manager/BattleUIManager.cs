#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 대미지나 효과등의 단발성 UI나 이펙트를 관리한다.
/// </summary>
public class BattleUIManager : BaseMonoManager<BattleUIManager>
{
    #region Property
    #endregion

    #region Value
    [SerializeField]
    private Canvas battleUICanvas;

    [SerializeField]
    private RectTransform battleUICanvasRect;

    [SerializeField]
    private HpBarUnit hpBarUnit;

    private Camera battleUICamera;
    private BattleInfo battleInfo;
    #endregion

    public async UniTask Prepare(BattleInfo battleInfo)
    {
        if (battleUICanvas != null)
            battleUICamera = battleUICanvas.worldCamera;

        this.battleInfo = battleInfo;
    }

    #region HPBar
    public async UniTask ShowHpBar()
    {
        var owner = battleInfo.CurrentCharacter.Model;

        if (hpBarUnit.Model == null)
            hpBarUnit.SetModel(new HpBarUnitModel());

        hpBarUnit.Hide();
        
        var model = hpBarUnit.Model;
        model.SetOwner(owner);
        model.SetBattleUICamera(battleUICamera);
        model.SetBattleUICanvas(battleUICanvasRect);

        await hpBarUnit.ShowAsync();
    }
    #endregion
}