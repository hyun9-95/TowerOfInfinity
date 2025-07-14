using System;

public class BattleViewModel : IBaseViewModel
{
    #region Property
    public int Level { get; private set; }
    public float BattleExp { get; private set; }
    public float NextBattleExp { get; private set; }
    public CharacterUnitModel[] CharacterUnitModels { get; private set; }
    public Action<int> OnChangeCharacter { get; private set; }
    #endregion

    #region Value
    public void SetCharacterUnitModels(CharacterUnitModel[] characterUnitModels)
    {
        CharacterUnitModels = characterUnitModels;
    }

    public void SetByBattleInfo(BattleInfo battleInfo)
    {
        SetLevel(battleInfo.Level);
        SetBattleExp(battleInfo.BattleExp);
        SetNextBattleExp(battleInfo.NextBattleExp);
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void SetNextBattleExp(float exp)
    {
        NextBattleExp = exp;
    }

    public void SetOnChangeCharacter(Action<int> action)
    {
        OnChangeCharacter = action;
    }
    #endregion
}
