#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public abstract class BaseFlow<T> : BaseFlow where T : IBaseFlowModel
{
    public T Model => GetModel<T>();
}

public abstract class BaseFlow
{
    public abstract UIType ViewType { get; }

    public abstract FlowType FlowType { get; }

    public virtual TransitionType TransitionType => TransitionType.Default;

    protected IBaseFlowModel baseModel;

    public T GetModel<T>() where T : IBaseFlowModel
    {
        return (T)baseModel;
    }

    public void SetModel(IBaseFlowModel model)
    {
        baseModel = model;
    }

    /// <summary>
    /// 시작 전 Transition In
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask LoadingProcess()
    {

    }

    /// <summary>
    /// 완료 후 Transition Out (Forget)
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Process()
    {

    }

    public virtual async UniTask Exit()
    {

    }
}

