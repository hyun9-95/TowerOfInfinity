using System;
using System.Collections.Generic;

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

    public Action<bool> OnShowHelmet { get; private set; }

    public Action<bool> OnShowEquipments { get; private set; }

    public Action OnChangeParts { get; set; }

    public Action OnCompleteCustomize { get; set; }
    #endregion

    #region Value

    #endregion

    #region Function
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

    public void SetIsShowHelmet(bool value)
    {
        IsShowHelmet = value;
    }

    public void SetIsShowEquipments(bool value)
    {
        IsShowEquipments = value;
    }
    #endregion
}
