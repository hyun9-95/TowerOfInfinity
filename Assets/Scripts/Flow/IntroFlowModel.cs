using UnityEngine;

public class IntroFlowModel : IBaseFlowModel
{
    public LoadDataType LoadDataType { get; private set; }

    public void SetLoadDataType(LoadDataType loadDataType)
    {
        LoadDataType = loadDataType;
    }
}
