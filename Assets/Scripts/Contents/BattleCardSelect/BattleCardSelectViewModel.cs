using System;
using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModels => cardUnitModels;

    public Action OnCompleteSelect { get; private set; }
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels;
    #endregion

    #region Function
    public void SetBattleCardUnitModels(List<BattleCardUnitModel> battleCardUnits)
    {
        cardUnitModels = battleCardUnits;
    }

    public void SetOnCompleteSelect(Action onCompleteSelect)
    {
        OnCompleteSelect = onCompleteSelect;
    }
    #endregion
}
