using UnityEngine;

public class CanvasCameraSupport : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UICanvasType uiCanvasType;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();
    }
#endif

    private void Start()
    {
        if (canvas == null)
            return;

        canvas.worldCamera = CameraManager.Instance.GetUICamera(uiCanvasType);
    }
}
