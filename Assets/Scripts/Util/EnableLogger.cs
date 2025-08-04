using UnityEngine;

public class EnableLogger : MonoBehaviour
{
    private void OnEnable()
    {
        Logger.Log($"[EnableLogger] OnEnable {gameObject.name}");
    }

    private void OnDisable()
    {
        Logger.Log($"[EnableLogger] OnDisable {gameObject.name}");
    }

    private void OnDestroy()
    {
        Logger.Log($"[EnableLogger] OnDestroy {gameObject.name}");
    }
}
