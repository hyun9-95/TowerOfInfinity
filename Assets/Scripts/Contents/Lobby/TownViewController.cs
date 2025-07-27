using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class TownViewController : BaseController<TownViewModel>
{
    public override UIType UIType => UIType.TownView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private TownView View => GetView<TownView>();

    public override void Enter()
    {
        Model.SetOnClickCustomization(OnClickCustomization);
	}

    private void OnClickCustomization()
    {
        var leaderCharacter = TownSceneManager.Instance.PlayerCharacter;
        var library = leaderCharacter.GetComponentInChildren<SpriteLibrary>();

        CharacterCustomizationController controller = new CharacterCustomizationController();
        CharacterCustomizationViewModel viewModel = new CharacterCustomizationViewModel();
        viewModel.SetPlayerSpriteLibrary(library);
        controller.SetModel(viewModel);
       

        UIManager.Instance.OpenPopup(controller).Forget();
    }
}
