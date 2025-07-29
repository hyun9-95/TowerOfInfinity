public class LocalizationManager : BaseManager<LocalizationManager>
{
    public LocalizationManager()
    {
        localContainer = DataManager.Instance.GetDataContainer<DataLocalization>();
    }

    private LocalizationType localizationType;
    private DataContainer<DataLocalization> localContainer;

    public void SetLocalizationType(LocalizationType type)
    {
        localizationType = type;
    }

    public string GetLocalization(LocalizationDefine define)
    {
        return GetLocalization(localContainer.GetById((int)define));
    }

    public string GetLocalization(DataLocalization dataLocalization)
    {
        return localizationType switch
        {
            LocalizationType.English => dataLocalization.English,
            LocalizationType.Korean => dataLocalization.Korean,
            LocalizationType.Chinese => dataLocalization.SimplifiedChinese,
            _ => null,
        };
    }

}