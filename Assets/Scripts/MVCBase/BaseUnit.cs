#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseUnit<T> : BaseUnit where T : IBaseUnitModel
{
    public T Model { get; private set; }

    public void SetModel(T model)
    {
        Model = model;
    }
}

public abstract class BaseUnit : AddressableMono
{
    public virtual void Refresh() { }

    public virtual void Show()
    {
        gameObject.SafeSetActive(true);
    }

    public virtual async UniTask ShowAsync()
    {
        gameObject.SafeSetActive(true);
    }
}

public class PoolableBaseUnit<T> : PoolableBaseUnit where T : IBaseUnitModel
{
    public T Model { get; private set; }
    public void SetModel(T model)
    {
        Model = model;
    }
}

public abstract class PoolableBaseUnit : TimedPoolableMono
{
    public virtual void Refresh() { }

    public virtual void Show()
    {
        gameObject.SafeSetActive(true);
    }

    public virtual async UniTask ShowAsync()
    {
        gameObject.SafeSetActive(true);
    }
}

