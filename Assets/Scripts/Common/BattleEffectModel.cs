using UnityEngine;

public class BattleEffectModel : IBaseUnitModel
{
	#region Property
	public Transform FollowTarget { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;
    }
    #endregion
}
