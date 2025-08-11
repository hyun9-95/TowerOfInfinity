using UnityEngine;

public class BattleObserverParam : IObserverParam
{
    #region Property
    public int IntValue { get; private set; }
    public float FloatValue { get; private set; }
    public CharacterUnitModel  ModelValue { get; private set; }
    #endregion

    #region Value

    #endregion

    #region Function
    public void SetIntValue(int value)
    {
        IntValue = value;
    }

    public void SetFloatValue(float value)
    {
        FloatValue = value;
    }

    public void SetModelValue(CharacterUnitModel modelValue)
    {
        ModelValue = modelValue;
    }
    #endregion

}
