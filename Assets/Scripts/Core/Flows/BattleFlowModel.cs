using UnityEngine;

public class BattleFlowModel : BaseFlowModel
{
    public DataDungeon DataDungeon { get; private set; }

    public void SetDataDungeon(DataDungeon dataEnemyGroup)
    {
        DataDungeon = dataEnemyGroup; 
    }
}
