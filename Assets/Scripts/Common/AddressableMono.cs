using UnityEngine;

public class AddressableMono : TrackableMono
{
    public override void OnDestroy()
    {
        base.OnDestroy();

        AddressableManager.Instance.ReleaseGameObject(gameObject);
    }
}

public class TrackableMono : MonoBehaviour
{
    public virtual void OnDestroy()
    {
        if (gameObject != null)
            AddressableManager.Instance.ReleaseTrackerAssets(this);
    }
}
