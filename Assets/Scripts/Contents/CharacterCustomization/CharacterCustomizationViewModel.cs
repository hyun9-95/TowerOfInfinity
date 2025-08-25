using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationViewModel : IBaseViewModel
{
    #region Property
    public bool IsShowHelmet { get; private set; }

    public bool IsShowEquipments { get; private set; }

    public CharacterRace SelectRace { get; private set; }

    public Dictionary<CharacterPartsType, DataCharacterParts> SelectRaceParts { get; private set; } = new Dictionary<CharacterPartsType, DataCharacterParts>();

    public CharacterRace[] SelectableRaces { get; private set; }

    public DataCharacterParts[] SelectableHairDatas { get; private set; }

    public DataCharacterParts SelectHairData { get; private set; }

    public Action<CharacterRace> OnSelectRace { get; private set; }

    public Action<DataCharacterParts> OnSelectHair { get; private set; }

    public Action<string> OnSelectHairColor { get; private set; }

    public Action<bool> OnShowHelmet { get; private set; }

    public Action<bool> OnShowEquipments { get; private set; }

    public Action OnChangeParts { get; private set; }

    public Action OnCompleteCustomize { get; private set; }

    public string HairPartsIconPath { get; private set; }
    public string HairPartsPreviewImagePath { get; private set; }

    public Dictionary<CharacterPartsType, string> ColorCodeDic { get; private set; } = new();
    public Dictionary<CharacterPartsType, Vector3> HsvDic { get; private set; } = new();
    #endregion

    #region Value

    #endregion

    #region Function
    public void SetSelectRace(CharacterRace race)
    {
        SelectRace = race;
    }

    public void SetOnSelectRace(Action<CharacterRace> onSelectRace)
    {
        OnSelectRace = onSelectRace;
    }

    public void SetOnSelectHair(Action<DataCharacterParts> onSelectHair)
    {
        OnSelectHair = onSelectHair;
    }

    public void SetOnCompleteCustomize(Action onComplete)
    {
        OnCompleteCustomize = onComplete;
    }

    public void SetSelectRaceParts(CharacterPartsType partsType, DataCharacterParts partsData)
    {
        SelectRaceParts[partsType] = partsData;
    }

    public void SetSelectHair(DataCharacterParts hairData)
    {
        SelectHairData = hairData;
    }

    public void SetSelectableHairDatas(DataCharacterParts[] hairDatas)
    {
        SelectableHairDatas = hairDatas;
    }

    public void SetSelectableRaces(CharacterRace[] selectableRaces)
    {
        SelectableRaces = selectableRaces;
    }

    public void SetOnShowHelemet(Action<bool> onShowHelemet)
    {
        OnShowHelmet = onShowHelemet;
    }

    public void SetOnShowEquipments(Action<bool> onShowEquipments)
    {
        OnShowEquipments = onShowEquipments;
    }

    public void SetOnSelectHairColor(Action<string> onSelectHairColor)
    {
        OnSelectHairColor = onSelectHairColor;
    }

    public void SetIsShowHelmet(bool value)
    {
        IsShowHelmet = value;
    }

    public void SetIsShowEquipments(bool value)
    {
        IsShowEquipments = value;
    }

    public void SetHairPartsIconPath(string path)
    {
        HairPartsIconPath = path;
    }

    public void SetHairPreviewImagePath(string path)
    {
        HairPartsPreviewImagePath = path;
    }
    #endregion
}
