public class ExpRangeUpBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function
    public override void OnStart()
    {
        BattleSystemManager.Instance.OnExpGainRangeUp();
    }

    public override void OnReapply()
    {
        OnStart();
    }
	#endregion
}
