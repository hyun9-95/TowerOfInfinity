using System;
using System.Collections.Generic;

public class UserSettingPopupModel : IBaseViewModel
{
    #region Property
    public Action<float> OnSetMasterVolume { get; private set; }

    public Action<SoundType, float> OnSetVolume { get; private set; }

    public Action<LocalizationType> OnChangeLocalizationType { get; private set; }

    public Action OnClickClose { get; private set; }

    public float MasterVolume { get; private set; }

    public IReadOnlyDictionary<SoundType, float> VolumeDic => volumeDic;

    public LocalizationType LocalizationType { get; private set; }
    #endregion

    #region Value
    private Dictionary<SoundType, float> volumeDic = new();
    #endregion

    #region Function
    public void SetOnClickClose(Action action)
    {
        OnClickClose = action;
    }

    public void SetLocalizationType(LocalizationType localizationType)
    {
        LocalizationType = localizationType;
    }

    public void SetOnMasterVolume(Action<float> onSetMasterVolume)
    {
        OnSetMasterVolume = onSetMasterVolume;
    }

    public void SetOnVolume(Action<SoundType, float> onSetVolume)
    {
        OnSetVolume = onSetVolume;
    }

    public void SetMasterVolume(float masterVolume)
    {
        MasterVolume = masterVolume;
    }

    public void SetVolume(SoundType soundType, float volume)
    {
        volumeDic[soundType] = volume;
    }

    public void SetOnChangeLocalizationType(Action<LocalizationType> onChnageLocalizationType)
    {
        OnChangeLocalizationType = onChnageLocalizationType;
    }
    #endregion
}
