public class UserSettingPopupController : BaseController<UserSettingPopupModel>
{
    public override UIType UIType => UIType.UserSettingPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

	public UserSettingPopup View => GetView<UserSettingPopup>();

    public override void Enter()
    {
        
    }
}
