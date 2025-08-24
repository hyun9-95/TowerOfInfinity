#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class InfiniteFollowColliderTriggerUnit : ColliderTriggerUnit
{
	#region Property
	#endregion
	
	#region Value
	#endregion
	
	#region Function
	protected override async UniTask EnableColliderAsync(float detectStartTime, float detectDuration)
	{
		hitCollider.enabled = true;
	}

    private void FixedUpdate()
    {
		if (Model.FollowTargetTransform == null)
			return;

		if (!Model.IsEnableFollow)
            gameObject.SafeSetActive(false);
    }
    #endregion
}
