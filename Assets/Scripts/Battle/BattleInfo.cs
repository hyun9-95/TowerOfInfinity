using UnityEngine;

public class BattleInfo
{

    #region Property
    public int Level { get; private set; }

    public float BattleExp { get; private set; }

    public float NextBattleExp
    {
        get
        {
            if (expTable.IsNull)
                return 0;

            int nextLevel = Level + 1;

            if (nextLevel >= expTable.ValueCount)
                return expTable.Values[expTable.ValueCount - 1];

            return expTable.Values[nextLevel];
        }
    }

    public BattleTeam BattleTeam { get; private set; }
    #endregion

    #region Value
    private DataBalance expTable;
    #endregion
    public void SetBattleTeam(BattleTeam battleTeam)
    {
        BattleTeam = battleTeam;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetBattleExp(float exp)
    {
        BattleExp = exp;
    }

    public void AddBattleExp(float exp)
    {
        BattleExp += exp;
    }

    public void SetExpTable()
    {
        expTable = DataManager.Instance.GetDataById<DataBalance>((int)BalanceDefine.BALANCE_BATTLE_EXP);
    }

    public void OnExpGain(float exp)
    {
        AddBattleExp(exp);

        if (BattleExp >= NextBattleExp)
        {
            Level++;
            BattleExp = 0;
        }
    }

    public void CreateBattleTeam()
    {

    }
}
