#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public abstract class BaseFlow<T> : BaseFlow where T : BaseFlowModel
{
    public T Model => GetModel<T>();

    protected Scene loadedScene;

    public override async UniTask LoadScene()
    {
        if (Model.SceneDefine == SceneDefine.None)
            return;

        loadedScene = await AddressableManager.Instance.LoadSceneAsync
           (Model.SceneDefine, LoadSceneMode.Single);

        if (!loadedScene.IsValid())
            return;
    }

    public override async UniTask UnloadScene()
    {
        if (Model.SceneDefine == SceneDefine.None)
            return;

        await AddressableManager.Instance.UnloadSceneAsync(Model.SceneDefine);
        loadedScene = default;
    }
}

public abstract class BaseFlow
{
    public abstract UIType ViewType { get; }

    public abstract FlowType FlowType { get; }

    public virtual TransitionType TransitionType => TransitionType.Default;

    protected BaseFlowModel baseModel;

    public T GetModel<T>() where T : BaseFlowModel
    {
        return (T)baseModel;
    }

    public void SetModel(BaseFlowModel model)
    {
        baseModel = model;
    }

    public virtual async UniTask LoadScene()
    {
    }

    public virtual async UniTask UnloadScene()
    {
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

