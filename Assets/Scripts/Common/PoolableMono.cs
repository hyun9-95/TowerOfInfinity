

public class PoolableMono : AddressableMono
{
    protected virtual void OnDisable()
    {
        TokenPool.Dispose(GetHashCode());
        ReturnPool();
    }

    private void ReturnPool()
    {
        ObjectPoolManager.Instance.ReturnToPool(gameObject, gameObject.name);
    }
}
