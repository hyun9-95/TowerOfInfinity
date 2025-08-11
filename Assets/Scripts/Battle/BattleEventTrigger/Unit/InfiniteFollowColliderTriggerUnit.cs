#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class InfiniteFollowColliderTriggerUnit : ColliderTriggerUnit
{
	#region Property
	#endregion
	
	#region Value
	#endregion
	
	#region Function
	protected override async UniTask EnableColliderAsync()
	{
		hitCollider.enabled = true;
	}

    private void FixedUpdate()
    {
		if (Model.FollowTarget == null)
			return;

		if (Model.FollowTarget.gameObject.activeSelf == false)
            gameObject.SafeSetActive(false);
    }
    #endregion
}
