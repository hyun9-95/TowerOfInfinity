using UnityEngine;

public class BattleObserverParam : IObserverParam
{
    #region Property
    public BattleInfo BattleInfo { get; private set; }
    public int IntValue { get; private set; }
    public float FloatValue { get; private set; }
    #endregion

    #region Value

    #endregion

    #region Function
    public void SetBattleInfo(BattleInfo battleInfo)
    {
        BattleInfo = battleInfo;
    }

    public void SetIntValue(int value)
    {
        IntValue = value;
    }

    public void SetFloatValue(float value)
    {
        FloatValue = value;
    }
    #endregion

}
