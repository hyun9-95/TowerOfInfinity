using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseMonoManager<CameraManager>
{
    [SerializeField]
    private Camera worldCamera;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;

    [SerializeField]
    private Camera[] uiCameras;

    public Camera GetWorldCamera()
    {
        return worldCamera;
    }

    public Vector3 GetBrainOutputPosition()
    {
        if (cinemachineBrain == null)
            return Vector3.zero;

        return cinemachineBrain.OutputCamera.transform.position;
    }
    public Camera GetUICamera(UICanvasType canvasType)
    {
        int index = (int)canvasType;

        if (index >= uiCameras.Length)
            return null;

        return uiCameras[index];
    }
}