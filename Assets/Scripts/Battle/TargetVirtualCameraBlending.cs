using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class TargetVirtualCameraBlending : AddressableMono
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private CinemachineCamera cinemachineCamera;
	#endregion
	
	#region Function
	public void SetTarget(Transform target)
	{
		cinemachineCamera.Follow = target;
    }

	public async UniTask StartBlending(float time, bool destroyAfterBlending)
	{
		await CameraManager.Instance.BlendingAsync(cinemachineCamera, time);

		if (destroyAfterBlending)
			GameObject.Destroy(gameObject);
	}
	#endregion
}
