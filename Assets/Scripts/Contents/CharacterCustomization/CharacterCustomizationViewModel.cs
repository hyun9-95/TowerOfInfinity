using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationViewModel : IBaseViewModel
{
    #region Property
    public bool IsShowHelmet { get; private set; }

    public bool IsShowEquipments { get; private set; }

    public CharacterRace SelectRace { get; private set; }

    public Dictionary<CharacterPartsType, CharacterPartsInfo> SelectRaceParts { get; private set; } = new Dictionary<CharacterPartsType, CharacterPartsInfo>();

    public CharacterRace[] SelectableRaces { get; private set; }

    public DataCharacterParts[] SelectableHairDatas { get; private set; }

    public CharacterPartsInfo SelectHairInfo { get; private set; }

    public CharacterPartsType CurrentEditingPartsType { get; private set; }

    public CharacterPartsInfo CurrentEditingPartsInfo { get; private set; }

    public Action<CharacterRace> OnSelectRace { get; private set; }

    public Action<DataCharacterParts> OnSelectHair { get; private set; }

    public Action<bool> OnShowHelmet { get; private set; }

    public Action<bool> OnShowEquipments { get; private set; }

    public Action<CharacterPartsType> OnSelectPartsForEdit { get; private set; }

    public Action<Color> OnChangeColor { get; private set; }

    public Action<Vector3> OnChangeHSV { get; private set; }

    public Action OnChangeParts { get; set; }

    public Action OnCompleteCustomize { get; set; }
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

    public void SetSelectRaceParts(CharacterPartsType partsType, CharacterPartsInfo partsInfo)
    {
        SelectRaceParts[partsType] = partsInfo;
    }

    public void SetSelectRaceParts(CharacterPartsType partsType, DataCharacterParts partsData)
    {
        SelectRaceParts[partsType] = new CharacterPartsInfo(partsData);
    }

    public void SetSelectHair(DataCharacterParts hairData)
    {
        SelectHairInfo = hairData != null ? new CharacterPartsInfo(hairData) : null;
    }

    public void SetSelectHair(CharacterPartsInfo hairInfo)
    {
        SelectHairInfo = hairInfo;
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

    public void SetIsShowHelmet(bool value)
    {
        IsShowHelmet = value;
    }

    public void SetIsShowEquipments(bool value)
    {
        IsShowEquipments = value;
    }

    public void SetOnSelectPartsForEdit(Action<CharacterPartsType> onSelectPartsForEdit)
    {
        OnSelectPartsForEdit = onSelectPartsForEdit;
    }

    public void SetOnChangeColor(Action<Color> onChangeColor)
    {
        OnChangeColor = onChangeColor;
    }

    public void SetOnChangeHSV(Action<Vector3> onChangeHSV)
    {
        OnChangeHSV = onChangeHSV;
    }

    public void SetCurrentEditingParts(CharacterPartsType partsType)
    {
        CurrentEditingPartsType = partsType;

        if (partsType == CharacterPartsType.Hair && SelectHairInfo != null)
        {
            CurrentEditingPartsInfo = SelectHairInfo;
        }
        else if (SelectRaceParts.TryGetValue(partsType, out var partsInfo))
        {
            CurrentEditingPartsInfo = partsInfo;
        }
        else
        {
            CurrentEditingPartsInfo = null;
        }
    }

    public void UpdateCurrentPartsColor(Color color)
    {
        if (CurrentEditingPartsInfo == null)
            return;

        CurrentEditingPartsInfo.ColorCode = $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }

    public void UpdateCurrentPartsHSV(Vector3 hsv)
    {
        if (CurrentEditingPartsInfo == null)
            return;

        CurrentEditingPartsInfo.HSV = hsv;
    }

    public Color GetCurrentPartsColor()
    {
        if (CurrentEditingPartsInfo == null || string.IsNullOrEmpty(CurrentEditingPartsInfo.ColorCode))
            return Color.white;

        if (ColorUtility.TryParseHtmlString(CurrentEditingPartsInfo.ColorCode, out var color))
            return color;

        return Color.white;
    }

    public Vector3 GetCurrentPartsHSV()
    {
        if (CurrentEditingPartsInfo == null)
            return Vector3.zero;

        return CurrentEditingPartsInfo.HSV;
    }
    #endregion
}
