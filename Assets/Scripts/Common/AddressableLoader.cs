using Cysharp.Threading.Tasks;
using UnityEngine;

public class AddressableLoader : AddressableMono
{
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private string address;
	#endregion

	#region Function
	public async UniTask<T> LoadAsync<T>() where T : UnityEngine.Object
	{
		return await AddressableManager.Instance.LoadAssetAsyncWithTracker<T>(address, this);
	}

	public async UniTask<T> InstantiateAsyc<T>() where T : AddressableMono
	{
		return await AddressableManager.Instance.InstantiateAddressableMonoAsync<T>(address, transform);
    }
    #endregion
}
