using UnityEngine;

public class AddressableMono : MonoBehaviour
{
    public virtual void OnDestroy()
    {
        Logger.Log($"{gameObject.name} destroyed.");

        if (gameObject != null)
            AddressableManager.Instance.ReleaseGameObject(gameObject);
    }
}
