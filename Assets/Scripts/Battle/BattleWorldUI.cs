#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 대미지나 효과등의 단발성 UI나 이펙트를 관리한다.
/// </summary>
public class BattleWorldUI : MonoBehaviour
{
    #region Property
    #endregion

    #region Value
    [SerializeField]
    private Canvas battleUICanvas;

    [SerializeField]
    private RectTransform battleUICanvasRect;

    [SerializeField]
    private AddressableLoader hpBarUnitLoader;

    private Camera battleUICamera;
    #endregion

    public async UniTask Prepare()
    {
        if (battleUICanvas != null)
            battleUICamera = battleUICanvas.worldCamera;
    }

    #region HPBar
    public async UniTask ShowHpBar(CharacterUnitModel owner)
    {
        if (owner == null)
            return;

        var hpBarUnit = await hpBarUnitLoader.InstantiateAsyc<HpBarUnit>();

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