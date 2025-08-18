using UnityEngine;

public class IntroFlowModel : BaseFlowModel
{
    public IntroFlowModel()
    {
        SetSceneDefine(SceneDefine.None);
    }

    public LoadDataType LoadDataType { get; private set; }

    public void SetLoadDataType(LoadDataType loadDataType)
    {
        LoadDataType = loadDataType;
    }
}
