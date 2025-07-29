using UnityEngine;

public class VolumeSetting : BaseSetting
{
    public float Bgm { get; private set; }
    public float Ambience { get; private set; }

    public override void Load()
    {
        Bgm = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_BGM_KEY), 0.5f);
        Ambience = PlayerPrefs.GetFloat(GetKey(PlayerPrefsDefine.VOLUME_AMBIENCE_KEY), 0.5f);
    }
  
    public void SetBgm()
    {

    }

    public void SetAmbience()
    {

    }
}
