using UnityEngine;

public class BattleFlowModel : IBaseFlowModel
{
    public SceneDefine BattleSceneDefine { get; private set; }

    public DataDungeon DataDungeon { get; private set; }

    public void SetBattleSceneDefine(SceneDefine define)
    {
        BattleSceneDefine = define;
    }

    public void SetDataDungeon(DataDungeon dataEnemyGroup)
    {
        DataDungeon = dataEnemyGroup; 
    }
}
