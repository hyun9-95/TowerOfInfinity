public class CustomizationFlowModel : BaseFlowModel
{
    public CustomizationFlowModel()
    {
        DataTown dataTown = DataManager.Instance.GetDataById<DataTown>((int)TownDefine.TOWN_CUSTOMIZATION);

        SetSceneDefine(dataTown.TownScene);
        SetFlowBGMPath(dataTown.BGM);
    }
}