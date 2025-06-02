public class BattleViewModel : IBaseViewModel
{
    #region Property
    public int Level { get; private set; }
    public float BattleExp { get; private set; }
    public float NextBattleExp { get; private set; }
    #endregion

    #region Value
    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void SetNextBattleExp(float exp)
    {
        NextBattleExp = exp;
    }
    #endregion
}
