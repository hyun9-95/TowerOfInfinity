using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCustomizationController : BaseController<CharacterCustomizationViewModel>
{
    public override UIType UIType => UIType.CharacterCustomizationView;
    public override UICanvasType UICanvasType => UICanvasType.View;

    private CharacterCustomizationView View => GetView<CharacterCustomizationView>();

    private MainPlayerCharacter mainPlayerCharacter;
    private DataContainer<DataCharacterParts> partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
    public override void Enter()
    {
        mainPlayerCharacter = PlayerManager.Instance.GetMainPlayerCharacter();
        partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();

        Model.SetOnCompleteCustomize(OnCompleteCustomize);
        Model.SetOnSelectRace(OnSelectRace);
        Model.SetOnSelectHair(OnSelectHair);
        Model.SetOnSelectHairColor(OnSelectHairColor);
        Model.SetOnShowHelemet(OnShowHelmet);
        Model.SetOnShowEquipments(OnShowEquipment);
        Model.SetSelectableRaces((CharacterRace[])Enum.GetValues(typeof(CharacterRace)));

        // 장비와 헬멧은 기본적으로 숨김
        Model.SetIsShowEquipments(false);
        Model.SetIsShowHelmet(false);

        var datas = partsContainer.FindAll(data => data.PartsType == CharacterPartsType.Hair);
        var selectableDatas = new List<DataCharacterParts>(datas.Length + 1);
        selectableDatas.Add(null);
        selectableDatas.AddRange(datas);

        Model.SetSelectableHairDatas(selectableDatas.ToArray());

        SetCurrentParts();
    }

    private void SetCurrentParts()
    {
        // 현재 메인캐릭터 기준 파츠데이터 삽입
        var mainCharacterInfo = PlayerManager.Instance.GetMainCharacterInfo();
        Model.SetSelectRace(mainCharacterInfo.CharacterRace);

        var getRaceParts = CommonUtils.GetRacePartsIds(Model.SelectRace);

        foreach (var partsId in getRaceParts)
        {
            var partsData = DataManager.Instance.GetDataContainer<DataCharacterParts>().GetById(partsId);
            Model.SetSelectRaceParts(partsData.PartsType, partsData);
        }

        var hairData = mainCharacterInfo.PartsInfo.GetPartsData(CharacterPartsType.Hair);
        Model.SetSelectHair(hairData);
    }

    public override async UniTask LoadingProcess()
    {
        if (mainPlayerCharacter == null)
            return;

        var libraryBuilder = mainPlayerCharacter.LibraryBuilder;
        libraryBuilder.Initialize();

        // 커스터마이즈 가능한 파츠들을 미리 로드
        libraryBuilder.SetMode(CharacterSpriteLibraryBuilder.Mode.Preload);
        await libraryBuilder.Preload();

        await OnChangePartsAsync(Model.IsShowEquipments, Model.IsShowHelmet);
    }

    private void OnSelectRace(CharacterRace characterRace)
    {
        var getRaceParts = CommonUtils.GetRacePartsIds(characterRace);

        foreach (var partsId in getRaceParts)
        {
            var partsData = DataManager.Instance.GetDataContainer<DataCharacterParts>().GetById(partsId);
            Model.SetSelectRaceParts(partsData.PartsType, partsData);
        }

        Model.SetSelectRace(characterRace);
        OnChangeParts();
    }

    private void OnSelectHair(DataCharacterParts hairData)
    {
        Model.SetSelectHair(hairData);
        OnChangeParts();
    }

    private void OnSelectHairColor(string hexCode)
    {
        var originhexCode = string.Empty;

        if (Model.ColorCodeDic.TryGetValue(CharacterPartsType.Hair, out var origin))
            originhexCode = origin;

        if (origin != hexCode)
        {
            Model.ColorCodeDic[CharacterPartsType.Hair] = hexCode;
            OnChangeParts();
        }
    }

    private void OnChangeParts()
    {
        OnChangePartsAsync(Model.IsShowEquipments, Model.IsShowHelmet).Forget();
    }

    private async UniTask OnChangePartsAsync(bool showEquipments, bool showHelmet)
    {
        if (mainPlayerCharacter == null)
            return;

        var changePartsList = Model.SelectRaceParts.Values.ToList();
        var removePartsList = new List<CharacterPartsType>();

        if (Model.SelectHairData != null)
        {
            changePartsList.Add(Model.SelectHairData);
        }
        else
        {
            removePartsList.Add(CharacterPartsType.Hair);
        }

        if (showEquipments)
        {
            var checkEquipmentTypes = new EquipmentType[]
            {
                EquipmentType.Weapon,
                EquipmentType.Bracers,
                EquipmentType.Shield,
                EquipmentType.Mask,
                EquipmentType.Armor,
            };

            foreach (var equipmentType in checkEquipmentTypes)
            {
                var equippedEquipment = PlayerManager.Instance.GetMainCharacterInfo().
                    GetEquippedEquipment(equipmentType);

                if (equippedEquipment != null)
                {
                    var partsData = partsContainer.GetById((int)equippedEquipment.PartsData);
                    changePartsList.Add(partsData);
                }
            }
        }

        if (showHelmet)
        {
            var equippedHelmet = PlayerManager.Instance.GetMainCharacterInfo().
                GetEquippedEquipment(EquipmentType.Helmet);

            if (equippedHelmet != null)
            {
                var partsData = partsContainer.GetById((int)equippedHelmet.PartsData);
                changePartsList.Add(partsData);
            }
        }
        else
        {
            removePartsList.Add(CharacterPartsType.Helmet);
        }

        List<CharacterPartsInfo> characterPartsInfos = new List<CharacterPartsInfo>();

        foreach (var parts in changePartsList)
        {
            string colorCode = string.Empty;
            Vector3 hsv = Vector3.zero;

            if (Model.ColorCodeDic.TryGetValue(parts.PartsType, out var partsColorCode))
                colorCode = partsColorCode;

            if (Model.HsvDic.TryGetValue(parts.PartsType, out var partsHsv))
                hsv = partsHsv;

            var newCharacterPartsInfo = new CharacterPartsInfo(parts, colorCode, hsv);
            characterPartsInfos.Add(newCharacterPartsInfo);
        }

        await mainPlayerCharacter.UpdateSpriteLibraryAsset(characterPartsInfos, removePartsList);
    }

    private void OnCompleteCustomize()
    {
        // 유저 정보에 반영
        var mainCharacterInfo = PlayerManager.Instance.GetMainCharacterInfo();
        mainCharacterInfo.SetCharacterRace(Model.SelectRace);
       
        // 파츠정보에 반영
        var partsInfo = mainCharacterInfo.PartsInfo;
        partsInfo.SetRaceParts(Model.SelectRace);
        partsInfo.SetHairParts(Model.SelectHairData != null ? Model.SelectHairData.Id : 0);
        partsInfo.SetShowHelmet(Model.IsShowHelmet);

        // 도입부 플래그
        PlayerManager.Instance.User.SetCompletePrologue(true);

        // 저장
        PlayerManager.Instance.User.Save();

        TownFlowModel townFlowModel = new TownFlowModel();

        var townData = PlayerManager.Instance.GetCurrentTownData();
        townFlowModel.SetDataTown(townData);
        townFlowModel.AddStateEvent(FlowState.TranstionIn, async() =>
        {
            await OnChangePartsAsync(true, Model.IsShowHelmet);
            mainPlayerCharacter.LibraryBuilder.ResetPreload();
        });

        FlowManager.Instance.ChangeFlow(FlowType.TownFlow, townFlowModel).Forget();
    }

    private void OnShowHelmet(bool value)
    {
        Model.SetIsShowHelmet(value);
        OnChangeParts();
    }

    private void OnShowEquipment(bool value)
    {
        Model.SetIsShowEquipments(value);
        OnChangeParts();
    }
}
