public class PoolableMono : AddressableMono
{
    /// <summary>
    /// 풀에 반환하므로, Override할 경우 맨 마지막에 호출해야한다.
    /// </summary>
    protected virtual void OnDisable()
    {
        TokenPool.Cancel(GetHashCode());
        ReturnPool();
    }

    private void ReturnPool()
    {
        if (ObjectPoolManager.Instance)
            ObjectPoolManager.Instance.ReturnToPool(gameObject, gameObject.name);
    }
}
