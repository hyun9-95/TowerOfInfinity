using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleCardSelectPopup : BaseView
{
    #region Property
    public BattleCardSelectViewModel Model => GetModel<BattleCardSelectViewModel>();
    #endregion

    #region Value
    [SerializeField]
    private BattleCardUnit[] battleCards;
    #endregion

    #region Function
    public override async UniTask ShowAsync()
    {
        await ShowCardUnits();
    }

    private async UniTask ShowCardUnits()
    {
        var tasks = new UniTask[battleCards.Length];
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (i < Model.CardUnitModelList.Count)
            {
                var card = battleCards[i];
                card.SetModel(Model.CardUnitModelList[i]);
                tasks[i] = card.ShowAsync();
            }
        }

        await UniTask.WhenAll(tasks);
    }

    #region OnEvent
    public void OnSelectCard(int index)
    {
        Model.OnClickBattleCard(index);
    }
    #endregion
    #endregion
}
