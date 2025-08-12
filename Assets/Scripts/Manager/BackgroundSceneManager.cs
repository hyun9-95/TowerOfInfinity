using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BackgroundSceneManager<T> : MonoBehaviour where T : BackgroundSceneManager<T>, new()
{
    private static T instance;

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
    public bool UseAStar => useAstar;
    #endregion

    #region Value
    [SerializeField]
    protected Transform playerTransform;

    [SerializeField]
    protected CinemachineCamera cinemachineCamera;

    [SerializeField]
    protected Transform objectContainer;

    [SerializeField]
    protected bool useAstar;
    #endregion

    public void SetFollowCamera(Transform target)
    {
        cinemachineCamera.Follow = target;
    }
}