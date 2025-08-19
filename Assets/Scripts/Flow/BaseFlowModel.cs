using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class BaseFlowModel
{
    #region Property
    public SceneDefine SceneDefine { get; private set; }

    public string FlowBGMPath {  get; private set; }
    #endregion

    #region Value
    private Dictionary<FlowState, List<Func<UniTask>>> stateEventDic = new();
    #endregion

    #region Function
    public void SetSceneDefine(SceneDefine define)
    {
        SceneDefine = define;
    }

    public void SetFlowBGMPath(string value)
    {
        FlowBGMPath = value;
    }

    public void AddStateEvent(FlowState state, Func<UniTask> stateEvent)
    {
        if (!stateEventDic.TryGetValue(state, out var list))
        {
            list = new List<Func<UniTask>>();
            stateEventDic[state] = list;
        }
        list.Add(stateEvent);
    }

    public void ClearStateEvent()
    {
        stateEventDic.Clear();
    }

    public bool IsExistStateEvent(FlowState state)
    {
        return stateEventDic.ContainsKey(state);
    }

    public async UniTask ProcessStateEvent(FlowState state)
    {
        if (!stateEventDic.TryGetValue(state, out var handlers))
            return;

        foreach (var handler in handlers)
            await handler();
    }
    #endregion
}
