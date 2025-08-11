using UnityEngine;

public class UserSettings
{
    public VolumeSetting Volume => volume;
    public LocalizationSetting Localization => localization;

    private VolumeSetting volume;
    private LocalizationSetting localization;

    public void LoadSettings(int id)
    {
        volume = new VolumeSetting();
        volume.SetPrefsKey(id);
        volume.Load();

        localization = new LocalizationSetting();
        localization.SetPrefsKey(id);
        localization.Load();
    }

    public float GetVolume(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Bgm:
                return volume.Bgm;

            case SoundType.Sfx:
                return volume.Sfx; 
        }

        return 1;
    }

    public LocalizationType GetLocalizationType()
    {
        return localization.Type;
    }
}

