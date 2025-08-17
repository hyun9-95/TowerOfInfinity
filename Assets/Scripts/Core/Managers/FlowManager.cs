#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlowManager : BaseManager<FlowManager>
{
    public FlowType CurrentFlowType => currentFlow.FlowType;

    private BaseFlow currentFlow;

    private BaseFlow prevFlow;

    public async UniTask ChangeFlow(FlowType flowType, BaseFlowModel baseFlowModel = null)
    {
        Logger.Log($"Change Flow {flowType}");

        var newFlow = FlowFactory.Create(flowType);
        newFlow.SetModel(baseFlowModel);

        if (newFlow == null)
            return;

        // Transition In
        await TransitionManager.Instance.In(newFlow.TransitionType);
        await ProcessStateEvent(FlowState.TranstionIn, baseFlowModel);

        if (currentFlow != null)
        {
            var prevType = currentFlow.FlowType;
            prevFlow = currentFlow;
            await prevFlow.Exit();
            await UIManager.Instance.ClearCurrentView();

            currentFlow = null;
            await CleanUpAsync();

            Logger.Log($"Exit Prev Flow {prevType} => For Change Flow {flowType}");
            await ProcessStateEvent(FlowState.ExitPrevFlow, baseFlowModel);
        }

        currentFlow = newFlow;
        await currentFlow.LoadScene();

        if (prevFlow != null)
            await prevFlow.UnloadScene();

        await currentFlow.LoadingProcess();
        await ProcessStateEvent(FlowState.LoadingProcess, baseFlowModel);

        await currentFlow.Process();
        await ProcessStateEvent(FlowState.Process, baseFlowModel);

        // Transition Out
        if (baseFlowModel.IsExistStateEvent(FlowState.TransitionOut))
        {
            await TransitionManager.Instance.Out(newFlow.TransitionType);
            await ProcessStateEvent(FlowState.TransitionOut, baseFlowModel);
        }
        else
        {
            TransitionManager.Instance.Out(newFlow.TransitionType).Forget();
        }

        baseFlowModel.ClearStateEvent();
    }

    public async UniTask ChangeCurrentTownFlow(SceneDefine sceneDefine)
    {
        var townFlowModel = new TownFlowModel();
        townFlowModel.SetSceneDefine(sceneDefine);

        await ChangeFlow(FlowType.TownFlow, townFlowModel);
    }

    private async UniTask ProcessStateEvent(FlowState state, BaseFlowModel flowModel)
    {
        await flowModel.ProcessStateEvent(state);
    }

    
    private async UniTask CleanUpAsync()
    {
        // 풀링해놨던 팩토리, 매니저 들을 Clear 해준다.
        ObjectPoolManager.Instance.Clear();
        CharacterFactory.Instance.Clear();

        AddressableManager.Instance.ReleaseAllHandles();
        await Resources.UnloadUnusedAssets();
    }
}