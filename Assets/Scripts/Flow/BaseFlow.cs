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

    protected IBaseFlowModel baseModel;

    public T GetModel<T>() where T : IBaseFlowModel
    {
        return (T)baseModel;
    }

    public void SetModel(IBaseFlowModel model)
    {
        baseModel = model;
    }

    public virtual async UniTask LoadingProcess()
    {

    }

    public virtual async UniTask Process()
    {

    }

    public virtual async UniTask Exit()
    {

    }
}

