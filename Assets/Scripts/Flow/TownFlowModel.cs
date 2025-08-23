using UnityEngine;

public class TownFlowModel : BaseFlowModel
{
    public DataTown DataTown { get; private set; }

    public void SetDataTown(DataTown dataTown)
    {
        DataTown = dataTown;

        SetSceneDefine(dataTown.TownScene);
        SetFlowBGMPath(dataTown.BGM);
    }
}
