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
    public CinemachineCamera CinemachineCamera => cinemachineCamera;
    public int ChunkSize => chunkSize;
    public bool UseAStar => walkableMaps != null;
    #endregion

    #region Value
    [SerializeField]
    protected Transform playerTransform;

    [SerializeField]
    protected CinemachineCamera cinemachineCamera;

    [SerializeField]
    protected Tilemap[] walkableMaps;

    [SerializeField]
    protected Tilemap[] obstacleMaps;

    [SerializeField]
    protected Grid layoutGrid;

    [SerializeField]
    private int chunkSize = 20;
    #endregion

    public void SetFollowCamera(Transform target)
    {
        cinemachineCamera.Follow = target;
    }
}