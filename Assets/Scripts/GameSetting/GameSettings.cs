using UnityEngine;

public class GameSettings
{
    public Volume Volume => volume;

    private Volume volume;

    public void LoadSettings()
    {
        volume = new Volume();
        volume.Load();
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
}


public partial class Volume
{
    public float Bgm { get; private set; }
    public float Ambience { get; private set; }

    public void Load()
    {
        Bgm = PlayerPrefs.GetFloat(PlayerPrefsDefine.VOLUME_BGM_KEY, 0.5f);
        Ambience = PlayerPrefs.GetFloat(PlayerPrefsDefine.VOLUME_BGM_KEY, 0.5f);
    }

    public void SetBgm()
    {

    }

    public void SetAmbience()
    {

    }
}

