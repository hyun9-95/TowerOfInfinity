using UnityEngine;

public class LocalizationSetting : BaseSetting
{
    public LocalizationType Type { get; set; }

    public override void Load()
    {
        Type = (LocalizationType)PlayerPrefs.GetInt(GetKey(PlayerPrefsDefine.LOCALIZATION_TYPE_KEY));
    }

    public void SaveLocalizationType(LocalizationType type)
    {
        PlayerPrefs.SetInt(GetKey(PlayerPrefsDefine.LOCALIZATION_TYPE_KEY), (int)type);
        Type = type;
    }
}
