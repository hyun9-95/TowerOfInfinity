using System;
using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModelList => cardUnitModels;

    public Action OnCompleteSelect { get; private set; }

    public Action<int> OnClickBattleCard { get; private set; }

    public Action<DataBattleCard> OnSelectBattleCard { get; private set; }
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels;
    #endregion

    #region Function
    public void SetBattleCardUnitModels(List<BattleCardUnitModel> cards)
    {
        cardUnitModels = cards;
    }

    public void SetOnSelectBattleCard(Action<DataBattleCard> onSelectCard)
    {
        OnSelectBattleCard = onSelectCard;
    }

    public void SetOnClickBattleCard(Action<int> onClickBattleCard)
    {
        OnClickBattleCard = onClickBattleCard;
    }

    public void SetOnCompleteSelect(Action onCompleteSelect)
    {
        OnCompleteSelect = onCompleteSelect;
    }
    #endregion
}
