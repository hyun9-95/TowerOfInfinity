#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public abstract class BaseView : AddressableMono
{
    protected IBaseViewModel BaseModel { get; private set; }

    public void SetModel(IBaseViewModel model)
    {
        BaseModel = model;
    }

    public T GetModel<T>() where T : IBaseViewModel
    {
        return (T)BaseModel;
    }

    public virtual async UniTask ShowAsync()
    {
        await UniTask.CompletedTask;
    }

    public virtual async UniTask HideAsync()
    {
    }
}
