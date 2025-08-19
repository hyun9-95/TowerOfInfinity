using UnityEngine;

public static class UserSettings
{
    public static VolumeSetting Volume => volume;
    public static LocalizationSetting Localization => localization;

    private static VolumeSetting volume;
    private static LocalizationSetting localization;

    public static void Load(string id)
    {
        volume = new VolumeSetting();
        volume.SetPrefsKey(id);
        volume.Load();

        localization = new LocalizationSetting();
        localization.SetPrefsKey(id);
        localization.Load();
    }

    public static float GetVolume(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Bgm:
                return volume.Bgm;

            case SoundType.Sfx:
                return volume.Sfx;

            case SoundType.Amb:
                return volume.Amb;
        }

        return 1;
    }

    public static LocalizationType GetLocalizationType()
    {
        return localization.Type;
    }
}

