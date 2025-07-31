using Cysharp.Threading.Tasks;
using System;

public class CharacterCustomizationController : BaseController<CharacterCustomizationViewModel>
{
    public override UIType UIType => UIType.CharacterCustomizationView;
    public override UICanvasType UICanvasType => UICanvasType.View;

    private CharacterCustomizationView View => GetView<CharacterCustomizationView>();

    public override void Enter()
    {
        InitializeModel();
        SetupModelEvents();
    }

    public override async UniTask Exit()
    {
        await base.Exit();
    }

    private void InitializeModel()
    {
        // 기본 파츠 설정 또는 저장된 사용자 데이터 로드
        var partsCount = Enum.GetNames(typeof(CharacterPartsType)).Length;
        Model.InitializeParts(partsCount);

        // 사용자 저장 데이터가 있다면 로드
        LoadUserCharacterData();
    }
    
    private void SetupModelEvents()
    {
        Model.SetOnPartChanged(OnPartChanged);
    }
    
    private void LoadUserCharacterData()
    {
    }

    private void OnPartChanged(int index, string value)
    {
        View.OnChangePart(index, value).Forget();
    }

    public void OnChangePart(int index, string value)
    {
        Model.ChangePart(index, value);
    }

    public void Save()
    {
        try
        {
            var userCharacterAppearanceInfo = new UserCharacterAppearanceInfo();
            userCharacterAppearanceInfo.parts = Model.Parts.ToArray();
            
            Logger.Log("Character customization saved successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to save character customization: {ex.Message}");
        }
    }
}
