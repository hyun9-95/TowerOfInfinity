public class PoolableMono : AddressableMono
{
    protected virtual void OnDisable()
    {
        TokenPool.Dispose(GetHashCode());
        ReturnPool();
    }

    private void ReturnPool()
    {
        if (ObjectPoolManager.Instance)
            ObjectPoolManager.Instance.ReturnToPool(gameObject, gameObject.name);
    }
}
