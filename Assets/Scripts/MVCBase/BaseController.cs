#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseController<T> : BaseController where T : IBaseViewModel
{
    protected T Model => GetModel<T>();

    public void SetModel(T model)
    {
        baseModel = model;
    }
}

public abstract class BaseController
{
    /// <summary> 프리팹 네이밍은 View와 동일하게.. ex)DataBoardController => DataBoardView </summary>
    public bool IsView => UICanvasType == UICanvasType.View;
    public abstract UIType UIType { get; }
    public abstract UICanvasType UICanvasType { get; }

    protected BaseView baseView;

    protected IBaseViewModel baseModel;

    public abstract void Enter();

    public T GetModel<T>() where T : IBaseViewModel
    {
        return (T)baseModel;
    }

    public T GetView<T>() where T : BaseView
    {
        return (T)baseView; 
    }

    public void SetView(BaseView view)
    {
        baseView = view;
        baseView.SetModel(baseModel);
    }

    public virtual async UniTask LoadingProcess()
    {

    }

    public virtual async UniTask Process()
    {
        await baseView.ShowAsync();
    }

    public virtual async UniTask Refresh()
    {
        await baseView.ShowAsync();
    }

    public virtual async UniTask Exit()
    {
        if (baseView.CheckSafeNull())
            return;

        GameObject.Destroy(baseView.gameObject);
    }
}
