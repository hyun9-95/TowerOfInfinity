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

    public void SetMasterVolume(float volume)
    {
        Master = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(GetKey(PlayerPrefsDefine.VOLUME_MASTAR_KEY), Master);
        PlayerPrefs.Save();
    }

    public void SetVolume(SoundType soundType, float volume)
    {
        switch (soundType)
        {
            case SoundType.Bgm:
                SetBgmVolume(volume);
                break;

            case SoundType.Amb:
                SetAmbVolume(volume);
                break;

            case SoundType.Sfx:
                SetSfxVolume(volume);
                break;
        }
    }

    private void SetBgmVolume(float volume)
    {
        Bgm = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(GetKey(PlayerPrefsDefine.VOLUME_BGM_KEY), Bgm);
        PlayerPrefs.Save();
    }

    private void SetSfxVolume(float volume)
    {
        Sfx = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(GetKey(PlayerPrefsDefine.VOLUME_SFX_KEY), Sfx);
        PlayerPrefs.Save();
    }

    private void SetAmbVolume(float volume)
    {
        Amb = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(GetKey(PlayerPrefsDefine.VOLUME_AMB_KEY), Amb);
        PlayerPrefs.Save();
    }
}
