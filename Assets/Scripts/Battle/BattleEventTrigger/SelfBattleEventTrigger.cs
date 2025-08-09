#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class SelfBattleEventTrigger : BattleEventTrigger
{
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function
    protected override async UniTask OnProcess()
    {
        SendBattleEventToTarget(Model.Sender);
    }
	#endregion
}
