using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseMonoManager<CameraManager>
{
    public float DiagonalLengthFromCenter => diagonalLengthFromCenter;

    [SerializeField]
    private Camera worldCamera;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;

    [SerializeField]
    private Camera[] uiCameras;

    private float diagonalLengthFromCenter;
    private float offset = 0.5f;

    private void Awake()
    {
        diagonalLengthFromCenter = CalculateCameraDiagonalLengthFromCenter();
        diagonalLengthFromCenter += offset;
    }

    public Camera GetWorldCamera()
    {
        return worldCamera;
    }

    public float CalculateCameraDiagonalLengthFromCenter()
    {
        Bounds camBounds = GetOrthographicCameraBounds(worldCamera);
        float width = camBounds.size.x;
        float height = camBounds.size.y;

        var diagonalLength = Mathf.Sqrt(width * width + height * height);

        // 중심점으로부터의 대각선 길이를 구해야되기 때문에 2로 나눠줌
        return diagonalLength / 2;
    }

    public Bounds GetOrthographicCameraBounds(Camera camera)
    {
        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        Vector3 center = camera.transform.position;
        center.z = 0f; // 2D니까..

        return new Bounds(center, new Vector3(width, height, 0f));
    }

    public Vector3 GetBrainOutputPosition()
    {
        if (cinemachineBrain == null)
            return Vector3.zero;

        return cinemachineBrain.OutputCamera.transform.position;
    }

    public Vector3 GetWorldToScreenPoint(Vector3 pos)
    {
        return worldCamera.WorldToScreenPoint(pos);
    }


    public Camera GetUICamera(UICanvasType canvasType)
    {
        int index = (int)canvasType;

        if (index >= uiCameras.Length)
            return null;

        return uiCameras[index];
    }
}