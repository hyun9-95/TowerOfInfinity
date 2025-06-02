using UnityEngine;

public class WorldToCanvasSupport : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    private Transform target;
    private RectTransform parentRect;

    private Camera worldCamera;
    private Camera uiCamera;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }
#endif

    private void Awake()
    {
        worldCamera = CameraManager.Instance.GetWorldCamera();
        uiCamera = CameraManager.Instance.GetUICamera(UICanvasType.View);
    }

    public void SetTarget(RectTransform parentRect, Transform target)
    {
        this.parentRect = parentRect;
        this.target = target;
    }

    public void SetParentRect(RectTransform parentRect)
    {
        this.parentRect = parentRect;
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        Vector2 screenPos =
            RectTransformUtility.WorldToScreenPoint(worldCamera, target.position);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenPos, uiCamera, out Vector2 local))
        {
            rectTransform.anchoredPosition = local;
        }
    }

    private void OnDisable()
    {
        parentRect = null;
        target = null;
    }
}
