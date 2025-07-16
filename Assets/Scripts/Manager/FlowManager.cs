#pragma warning disable CS1998

using Cysharp.Threading.Tasks;

public class FlowManager : BaseManager<FlowManager>
{
    public FlowType CurrentFlowType => currentFlow.FlowType;

    private BaseFlow currentFlow;

    private BaseFlow prevFlow;

    public async UniTask ChangeFlow(FlowType flowType, IBaseFlowModel baseFlowModel = null)
    {
        Logger.Log($"Change Flow {flowType}");

        if (currentFlow != null)
        {
            var prevType = currentFlow.FlowType;
            prevFlow = currentFlow;
            await prevFlow.Exit();
            currentFlow = null;

            Logger.Log($"Exit Prev Flow {prevType} => For Change Flow {flowType}");
        }

        var newFlow = FlowFactory.Create(flowType);
        newFlow.SetModel(baseFlowModel);

        if (newFlow == null)
            return;

        currentFlow = newFlow;

        await TransitionManager.Instance.In(newFlow.TransitionType);
        await currentFlow.LoadingProcess();
        await currentFlow.Process();
        TransitionManager.Instance.Out(newFlow.TransitionType).Forget();
    }
}