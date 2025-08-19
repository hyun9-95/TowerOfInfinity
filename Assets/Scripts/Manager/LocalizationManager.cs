public class LocalizationManager : BaseManager<LocalizationManager>
{
    private LocalizationType localizationType;
    private DataContainer<DataLocalization> localContainer;

    public void SetLocalizationType(LocalizationType localizationType)
    {
        this.localizationType = localizationType;
    }

    public void Initialize()
    {
        localContainer = DataManager.Instance.GetDataContainer<DataLocalization>();
    }

    public static string GetLocalization(LocalizationDefine define)
    {
        return GetLocalization(Instance.localContainer.GetById((int)define));
    }

    public static string GetLocalization(DataLocalization dataLocalization)
    {
        return Instance.localizationType switch
        {
            LocalizationType.English => dataLocalization.English,
            LocalizationType.Korean => dataLocalization.Korean,
            LocalizationType.Chinese => dataLocalization.SimplifiedChinese,
            _ => null,
        };
    }

}