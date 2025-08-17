using Unity.Cinemachine;
using UnityEngine;

public abstract class BackgroundSceneManager<T> : MonoBehaviour where T : BackgroundSceneManager<T>, new()
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<T>();

            return instance;
        }
    }

    #region Property
    public Transform PlayerStartTransform => playerTransform;
    public Transform ObjectContainter => objectContainer;
    public CinemachineCamera CinemachineCamera => cinemachineCamera;
    #endregion

    #region Value
    [SerializeField]
    protected Transform playerTransform;

    [SerializeField]
    protected CinemachineCamera cinemachineCamera;

    [SerializeField]
    protected Transform objectContainer;
    #endregion

    public void SetFollowCamera(Transform target)
    {
        cinemachineCamera.Follow = target;
    }
}