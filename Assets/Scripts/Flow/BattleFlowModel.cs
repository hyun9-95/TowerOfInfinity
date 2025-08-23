using UnityEngine;

public class BattleFlowModel : BaseFlowModel
{
    public DataDungeon DataDungeon { get; private set; }

    public void SetDataDungeon(DataDungeon dataDungeon)
    {
        DataDungeon = dataDungeon;
        SetSceneDefine(dataDungeon.BattleScene);
        SetFlowBGMPath(dataDungeon.BGM);
    }
}
