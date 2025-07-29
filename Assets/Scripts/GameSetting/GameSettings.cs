using UnityEngine;

public class GameSettings
{
    public VolumeSetting Volume => volume;
    public LocalizationSetting Localization => localization;

    private VolumeSetting volume;
    private LocalizationSetting localization;

    public void LoadSettings()
    {
        volume = new VolumeSetting();
        volume.Load();

        localization = new LocalizationSetting();
        localization.Load();
    }

    public float GetVolume(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Bgm:
                return volume.Bgm;

            case SoundType.Ambience:
                return volume.Ambience; 
        }

        return 1;
    }

    public LocalizationType GetLocalizationType()
    {
        return localization.Type;
    }
}

