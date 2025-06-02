using Cysharp.Threading.Tasks;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    public delegate void RefreshSafeArea();
    public static RefreshSafeArea OnEventRefreshSafeArea;

    private RectTransform rectTransform;
    private bool isInitialize = true;

    private void Awake()
    {
        OnEventRefreshSafeArea += Refresh;
        SetArea();
    }

    private void OnDestroy()
    {
        OnEventRefreshSafeArea -= Refresh;
    }

    private async void Refresh()
    {
        try
        {
            await UniTask.NextFrame();

            if (!this)
                return;

            if (gameObject == null || gameObject.Equals(null))
                return;

            SetArea();
        }
        catch
        {
        }
    }

    private void OnEnable()
    {
        if (isInitialize)
            return;

        SetArea();
    }

    [ContextMenu("SetArea")]
    private void SetArea()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;

        isInitialize = false;
    }
}