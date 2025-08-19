using UnityEngine;

public class VolumeSetting : BaseSetting
{
    public float Master { get; private set; }
    public float Bgm { get; private set; }
    public float Sfx { get; private set; }
    public float Amb { get; private set; }

    public override void Load()
    {
        Master = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_MASTAR_KEY), 0.5f);
        Bgm = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_BGM_KEY), 1f);
        Sfx = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_SFX_KEY), 1f);
        Amb = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_AMB_KEY), 1f);
    }
}
