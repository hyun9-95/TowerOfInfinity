using Cysharp.Threading.Tasks;
using System;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class CharacterCustomizationController : BaseController<CharacterCustomizationViewModel>
{
    public override UIType UIType => UIType.CharacterCustomizationView;
    public override UICanvasType UICanvasType => UICanvasType.Popup;

    private CharacterCustomizationView View => GetView<CharacterCustomizationView>();

    public override void Enter()
    {
        InitializeModel();
        SetupModelEvents();
    }

    public override async UniTask Exit()
    {
        CleanupModelEvents();
        await base.Exit();
    }

    private void InitializeModel()
    {
        // 기본 파츠 설정 또는 저장된 사용자 데이터 로드
        var partsCount = Enum.GetNames(typeof(CharacterPartsName)).Length;
        Model.InitializeParts(partsCount);

        // 사용자 저장 데이터가 있다면 로드
        LoadUserCharacterData();
    }
    
    private void SetupModelEvents()
    {
        Model.OnPartChanged += OnPartChanged;
    }
    
    private void CleanupModelEvents()
    {
        Model.OnPartChanged -= OnPartChanged;
    }

    private void LoadUserCharacterData()
    {
        // 저장된 사용자 캐릭터 데이터 로드 로직
        // 예: PlayerManager.Instance.GetUserCharacterAppearance()
        // 현재는 기본값 사용
    }

    private async void OnPartChanged(int index, string value)
    {
        // Model에서 파츠가 변경되었을 때 View에 알림
        try
        {
            await View.OnChangePart(index, value);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to handle part change: {ex.Message}");
        }
    }

    public void OnChangePart(int index, string value)
    {
        // 이 메서드는 View에서 직접 Model을 업데이트하므로 필요시에만 사용
        Model.ChangePart(index, value);
    }

    public void Save()
    {
        try
        {
            var userCharacterAppearanceInfo = new UserCharacterAppearanceInfo();
            userCharacterAppearanceInfo.parts = Model.Parts.ToArray();
            
            // 데이터 저장 로직
            // 예: PlayerManager.Instance.SaveUserCharacterAppearance(userCharacterAppearanceInfo)
            
            Logger.Log("Character customization saved successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to save character customization: {ex.Message}");
        }
    }
}
