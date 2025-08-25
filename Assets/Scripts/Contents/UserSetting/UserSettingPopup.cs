#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserSettingPopup : BaseView
{
    public UserSettingPopupModel Model => GetModel<UserSettingPopupModel>();

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider bgmVolumeSlider;

    [SerializeField]
    private Slider ambVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private TMP_Dropdown localizationDropDown;

    public override async UniTask ShowAsync()
    {
        SetUpSliderEvents();
        SetUpLocalizationDropdown();

        RefreshUI();
    }

    private void SetUpSliderEvents()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(value => OnVolumeChanged(SoundType.Bgm, value));
        ambVolumeSlider.onValueChanged.AddListener(value => OnVolumeChanged(SoundType.Amb, value));
        sfxVolumeSlider.onValueChanged.AddListener(value => OnVolumeChanged(SoundType.Sfx, value));
    }

    private void SetUpLocalizationDropdown()
    {
        if (localizationDropDown == null)
            return;

        localizationDropDown.options.Clear();
        var localizationTypes = Enum.GetValues(typeof(LocalizationType)).Cast<LocalizationType>().Where(x => x != LocalizationType.None);
        
        foreach (var type in localizationTypes)
        {
            localizationDropDown.options.Add(new TMP_Dropdown.OptionData(type.ToString()));
        }

        localizationDropDown.onValueChanged.AddListener(OnLocalizationChanged);
    }

    private void RefreshUI()
    {
        masterVolumeSlider.value = Model.MasterVolume;
        
        if (Model.VolumeDic.ContainsKey(SoundType.Bgm))
            bgmVolumeSlider.value = Model.VolumeDic[SoundType.Bgm];
        
        if (Model.VolumeDic.ContainsKey(SoundType.Amb))
            ambVolumeSlider.value = Model.VolumeDic[SoundType.Amb];
        
        if (Model.VolumeDic.ContainsKey(SoundType.Sfx))
            sfxVolumeSlider.value = Model.VolumeDic[SoundType.Sfx];

        RefreshLocalizationDropdown();
    }

    private void RefreshLocalizationDropdown()
    {
        if (localizationDropDown == null)
            return;

        var localizationTypes = Enum.GetValues(typeof(LocalizationType)).Cast<LocalizationType>().Where(x => x != LocalizationType.None).ToArray();
        int currentIndex = Array.IndexOf(localizationTypes, Model.LocalizationType);
        
        if (currentIndex >= 0)
            localizationDropDown.value = currentIndex;
    }

    private void OnMasterVolumeChanged(float value)
    {
        Model.OnSetMasterVolume?.Invoke(value);
    }

    private void OnVolumeChanged(SoundType soundType, float value)
    {
        Model.OnSetVolume?.Invoke(soundType, value);
    }

    private void OnLocalizationChanged(int index)
    {
        var localizationTypes = Enum.GetValues(typeof(LocalizationType)).Cast<LocalizationType>().Where(x => x != LocalizationType.None).ToArray();
        
        if (index >= 0 && index < localizationTypes.Length)
        {
            LocalizationType selectedType = localizationTypes[index];
            Model.OnChangeLocalizationType?.Invoke(selectedType);
        }
    }

    public void OnClickClose()
    {
        Model.OnClickClose?.Invoke();
    }
}
