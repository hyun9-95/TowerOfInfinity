using UnityEngine;

public class VolumeSetting : BaseSetting
{
    public float Bgm { get; private set; }
    public float Sfx { get; private set; }
    public float Amb { get; private set; }

    public override void Load()
    {
        Bgm = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_BGM_KEY), 0.5f);
        Sfx = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_SFX_KEY), 0.5f);
        Amb = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_AMB_KEY), 0.5f);
    }
  
    public void SetBgm()
    {

    }

    public void SetAmbience()
    {

    }
}
