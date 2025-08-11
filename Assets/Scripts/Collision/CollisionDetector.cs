using UnityEngine;


public class CollisionDetector<T> : CollisionDetector where T: CollisionDetectorModel, new()
{
    public T Model => GetModel<T>();
}

public class CollisionDetector : AddressableMono
{
    protected CollisionDetectorModel baseModel;

    [SerializeField]
    protected bool isOneTime = false;

    public T GetModel<T>() where T : CollisionDetectorModel, new()
    {
        if (baseModel == null)
            baseModel = new T();

        return (T)baseModel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnterCollision(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExitCollision(collision);
    }

    protected virtual void OnEnterCollision(Collider2D collision)
    {
        if (baseModel == null)
            return;

        baseModel.OnTriggerEnter?.Invoke(collision);

        if (isOneTime)
            baseModel.OnTriggerEnter = null;
    }

    protected virtual void OnExitCollision(Collider2D collision)
    {
        if (baseModel == null)
            return;

        baseModel.OnTriggerExit?.Invoke(collision);
    }
}
