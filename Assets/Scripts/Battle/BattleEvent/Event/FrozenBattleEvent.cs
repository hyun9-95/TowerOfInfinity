using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class FrozenBattleEvent : BattleEvent
{
    #region Property
    #endregion

    #region Value
    private CancellationTokenSource frozenTokenSource;
    #endregion

    #region Function
    public override void OnStart()
    {
        frozenTokenSource = new CancellationTokenSource();

        Model.Receiver.ActionHandler.OnBodyColorChange(GetFrozenBodyColor(), frozenTokenSource.Token).Forget();
        Model.Receiver.ActionHandler.OnCCStart(BattleEventType.Frozen);

        DelayDamage();
    }

    private void DelayDamage()
    {
        UniTaskUtils.DelayAction(1f, () => BattleSystemManager.OnDamage
            (Model.Sender, Model.Receiver, GetAppliableStatValue()), frozenTokenSource.Token).Forget();
    }

    public override void OnEnd()
    {
        frozenTokenSource.Cancel();
        Model.Receiver.ActionHandler.OnCCEnd(BattleEventType.Frozen);
    }

    private Color GetFrozenBodyColor()
    {
        return new Color32(0, 100, 255, 255);
    }
	#endregion
}
