using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModels => cardUnitModels;
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels = new List<BattleCardUnitModel>();
    #endregion

    #region Function

    #endregion
}
