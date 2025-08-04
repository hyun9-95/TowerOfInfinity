using Cysharp.Threading.Tasks;
using System.Linq;
using System;

public class CharacterCustomizationController : BaseController<CharacterCustomizationViewModel>
{
    public override UIType UIType => UIType.CharacterCustomizationView;
    public override UICanvasType UICanvasType => UICanvasType.View;

    private CharacterCustomizationView View => GetView<CharacterCustomizationView>();

    private MainPlayerCharacter mainPlayerCharacter;

    public override void Enter()
    {
        Model.SetOnCompleteCustomize(OnCompleteCustomize);
        Model.SetOnSelectRace(OnSelectRace);
        Model.SetOnSelectHair(OnSelectHair);
        Model.SetSelectableRaces((CharacterRace[])Enum.GetValues(typeof(CharacterRace)));

        var partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
        var datas = partsContainer.FindAll(data => data.PartsType == CharacterPartsType.Hair);
        Model.SetSelectableHairDatas(datas);

        SetCurrentParts();
    }

    private void SetCurrentParts()
    {
        mainPlayerCharacter = PlayerManager.Instance.GetMainPlayerCharacter();

        // 현재 메인캐릭터 기준 파츠데이터 삽입
        var mainCharacterInfo = PlayerManager.Instance.GetMainCharacterInfo();

        var customizablePartsType = new CharacterPartsType[]
        {
            CharacterPartsType.Head,
            CharacterPartsType.Body,
            CharacterPartsType.Ears,
            CharacterPartsType.Eyes,
            CharacterPartsType.Hair,
        };

        foreach (var partsType in customizablePartsType)
        {
            var partsData = mainCharacterInfo.PartsInfo.GetPartsData(partsType);
            Model.SetSelectRaceParts(partsType, partsData);
        }
    }

    public override async UniTask LoadingProcess()
    {
        if (mainPlayerCharacter == null)
            return;

        var libraryBuilder = mainPlayerCharacter.LibraryBuilder;

        // 커스터마이즈 가능한 파츠들을 미리 로드
        libraryBuilder.SetMode(CharacterSpriteLibraryBuilder.Mode.Preload);
        await libraryBuilder.Preload();
    }

    public override async UniTask Exit()
    {
        await base.Exit();
    }

    private void OnSelectRace(CharacterRace characterRace)
    {
        var getRaceParts = CommonUtils.GetRacePartsIds(characterRace);

        foreach (var partsId in getRaceParts)
        {
            var partsData = DataManager.Instance.GetDataContainer<DataCharacterParts>().GetById(partsId);
            Model.SetSelectRaceParts(partsData.PartsType, partsData);
        }

        OnChangeParts();
    }

    private void OnSelectHair(DataCharacterParts hairData)
    {
        Model.SetSelectHair(hairData);
        OnChangeParts();
    }

    private void OnChangeParts()
    {
        if (mainPlayerCharacter == null)
            return;

        var changePartsList = Model.SelectRaceParts.Values.ToList();
        changePartsList.Add(Model.SelectHairData);

        mainPlayerCharacter.UpdateSpriteLibraryAsset(changePartsList).Forget();
    }

    private void OnCompleteCustomize()
    {
        var mainCharacterInfo = PlayerManager.Instance.GetMainCharacterInfo();
        var partsInfo = mainCharacterInfo.PartsInfo;

        partsInfo.SetRaceParts(Model.SelectRace);
        partsInfo.SetHairParts(Model.SelectHairData.Id);

        PlayerManager.Instance.MyUser.Save();

        FlowManager.Instance.ChangeFlow(FlowType.TownFlow).Forget();
    }
}
