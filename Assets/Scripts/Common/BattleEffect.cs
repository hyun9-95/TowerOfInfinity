using UnityEngine;

public class BattleEffect : PoolableBaseUnit<BattleEffectModel>
{
    #region Property
    public bool IsFollowTarget => isFollowTarget;
    #endregion

    #region Value
    [SerializeField]
    private bool isFollowTarget = false;
    #endregion

    #region Function
    private void Update()
    {
        if (Model == null)
            return;

        if (Model.FollowTarget == null)
            return;

        transform.position = Model.FollowTarget.position;
        transform.localPosition += LocalPosOffset;
    }
    #endregion
}
