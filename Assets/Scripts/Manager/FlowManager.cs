#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlowManager : BaseManager<FlowManager>
{
    public FlowType CurrentFlowType => currentFlow.FlowType;

    private BaseFlow currentFlow;

    private BaseFlow prevFlow;

    public async UniTask ChangeFlow(FlowType flowType, IBaseFlowModel baseFlowModel = null)
    {
        Logger.Log($"Change Flow {flowType}");

        var newFlow = FlowFactory.Create(flowType);
        newFlow.SetModel(baseFlowModel);

        if (newFlow == null)
            return;

        // Transition In
        await TransitionManager.Instance.In(newFlow.TransitionType);

        if (currentFlow != null)
        {
            var prevType = currentFlow.FlowType;
            prevFlow = currentFlow;
            await prevFlow.Exit();
            currentFlow = null;

            AddressableManager.Instance.ReleaseAllHandles();
            await Resources.UnloadUnusedAssets();
            Logger.Log($"Exit Prev Flow {prevType} => For Change Flow {flowType}");
        }

        currentFlow = newFlow;
        await currentFlow.LoadingProcess();
        await currentFlow.Process();

        // Transition Out
        TransitionManager.Instance.Out(newFlow.TransitionType).Forget();
    }
}