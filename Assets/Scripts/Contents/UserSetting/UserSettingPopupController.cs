using Cysharp.Threading.Tasks;

public class UserSettingPopupController : BaseController<UserSettingPopupModel>
{
    public override UIType UIType => UIType.UserSettingPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

	public UserSettingPopup View => GetView<UserSettingPopup>();

    public override void Enter()
    {
        Model.SetOnMasterVolume(OnMasterVolumeChange);
        Model.SetOnVolume(OnVolumeChange);
        Model.SetOnChangeLocalizationType(OnChangeLocalizationType);
        Model.SetOnClickClose(OnClickClose);
    }

    private void OnMasterVolumeChange(float value)
    {
        UserSettings.Volume.SetMasterVolume(value);
        SoundManager.Instance.UpdateVolume();
    }

    private void OnVolumeChange(SoundType soundType, float value)
    {
        UserSettings.Volume.SetVolume(soundType, value);
        SoundManager.Instance.UpdateVolume();
    }

    private void OnChangeLocalizationType(LocalizationType localizationType)
    {
        if (Model.LocalizationType == localizationType)
            return;

        Model.SetLocalizationType(localizationType);

        UserSettings.Localization.SaveLocalizationType(localizationType);
        LocalizationManager.Instance.SetLocalizationType(localizationType);

        ObserverManager.NotifyObserver(LocalizationObserverID.Changed, null);
    }

    private void OnClickClose()
    {
        UIManager.Instance.Back().Forget();
    }
}
