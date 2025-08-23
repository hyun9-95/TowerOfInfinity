public class ExpRangeUpBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function
    public override void OnStart()
    {
        BattleApi.OnExpGainRangeUp();
    }

    public override void OnReapply()
    {
        OnStart();
    }
	#endregion
}
