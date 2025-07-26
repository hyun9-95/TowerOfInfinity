using UnityEngine;

public class AddressableMono : MonoBehaviour
{
    public virtual void OnDestroy()
    {
        if (gameObject != null)
            AddressableManager.Instance.ReleaseGameObject(gameObject);
    }
}
