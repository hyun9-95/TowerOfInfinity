using System.Collections.Generic;

public class BattleInfoParam
{
	#region Property
	#endregion

	#region Value
	public int Level;
	public float BattleExp;
	public float NextBattleExp;
	public int KillCount;
	public int CurrentWave;
	public float BattleStartTime;
	public BattleState BattleState;
	public IReadOnlyDictionary<AbilitySlotType, List<Ability>> AbilitySlotDic;
	#endregion

	#region Function
	public BattleInfoParam(BattleInfo battleInfo)
	{
        Level = battleInfo.Level;
        BattleExp = battleInfo.BattleExp;
        NextBattleExp = battleInfo.NextBattleExp;
        KillCount = battleInfo.KillCount;
        CurrentWave = battleInfo.CurrentWave;
        BattleStartTime = battleInfo.BattleStartTime;
        BattleState = battleInfo.BattleState;
    }
	#endregion
}
