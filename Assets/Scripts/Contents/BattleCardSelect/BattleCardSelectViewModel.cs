using System;
using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModels => cardUnitModels;

    public Action OnCompleteSelect { get; private set; }
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels = new List<BattleCardUnitModel>();
    #endregion

    #region Function
    public void AddBattleCardUnitModel(BattleCardUnitModel model)
    {
        cardUnitModels.Add(model);
    }

    public void SetOnCompleteSelect(Action onCompleteSelect)
    {
        OnCompleteSelect = onCompleteSelect;
    }
    #endregion
}
