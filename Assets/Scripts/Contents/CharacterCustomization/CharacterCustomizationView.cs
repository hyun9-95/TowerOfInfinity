#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationView : BaseView
{
    public CharacterCustomizationViewModel Model => GetModel<CharacterCustomizationViewModel>();

    [SerializeField]
    private TMP_Dropdown raceDropDown;

    [SerializeField]
    private TMP_Dropdown hairDropDown;

    [SerializeField]
    private ColorSelectUnit hairColorSelectUnit;

    [SerializeField]
    private Toggle showHelmentToggle;

    [SerializeField]
    private Toggle showEquipmentToggle;

    [SerializeField]
    private Button completeButton;

    private void Awake()
    {
        SetUpEventListeners();
    }

    public override async UniTask ShowAsync()
    {
        SetUpDropdownOptions();
        SetUpToggleValues();
        await SetUpHairColorSelectUnit();
    }

    public async UniTask RefreshHairPartsIconPath()
    {
        if (hairColorSelectUnit == null || hairColorSelectUnit.Model == null)
            return;

        hairColorSelectUnit.Model.SetPartsImagePath(Model.HairPartsIconPath);
        hairColorSelectUnit.Model.SetPreviewImagePath(Model.HairPartsPreviewImagePath);

        await hairColorSelectUnit.UpdatePartsImage();
    }

    private void SetUpEventListeners()
    {
        if (raceDropDown != null)
            raceDropDown.onValueChanged.AddListener(OnRaceDropdownChanged);

        if (hairDropDown != null)
            hairDropDown.onValueChanged.AddListener(OnHairDropdownChanged);

        if (showHelmentToggle != null)
            showHelmentToggle.onValueChanged.AddListener(OnShowHelmetToggleChanged);

        if (showEquipmentToggle != null)
            showEquipmentToggle.onValueChanged.AddListener(OnShowEquipmentToggleChanged);

        if (completeButton != null)
            completeButton.onClick.AddListener(OnCompleteCustomize);
    }

    private void SetUpDropdownOptions()
    {
        SetupRaceDropdownOptions();
        SetupHairDropdownOptions();
        SetCurrentDropdownValues();
    }

    private void SetupRaceDropdownOptions()
    {
        if (raceDropDown == null || Model.SelectableRaces == null)
            return;

        raceDropDown.ClearOptions();
        
        var raceOptions = Model.SelectableRaces.Select(race => race.GetLocalization()).ToList();
        raceDropDown.AddOptions(raceOptions);
    }

    private void SetupHairDropdownOptions()
    {
        if (hairDropDown == null || Model.SelectableHairDatas == null)
            return;

        hairDropDown.ClearOptions();
        
        var hairOptions = Model.SelectableHairDatas.Select(hairData => 
            hairData == null ? "None" : 
            hairData.PartsName.Substring(hairData.PartsName.LastIndexOf('/') + 1)).ToList();
        hairDropDown.AddOptions(hairOptions);
    }

    private void SetUpToggleValues()
    {
        if (showHelmentToggle != null)
            showHelmentToggle.isOn = Model.IsShowHelmet;

        if (showEquipmentToggle != null)
            showEquipmentToggle.isOn = Model.IsShowEquipments;
    }

    private async UniTask SetUpHairColorSelectUnit()
    {
        if (hairColorSelectUnit == null)
            return;

        ColorSelectUnitModel colorSelectUnitModel = new();
        colorSelectUnitModel.SetOnColorConfirmed(OnHairColorChanged);
        colorSelectUnitModel.SetPartsImagePath(Model.HairPartsIconPath);
        colorSelectUnitModel.SetPreviewImagePath(Model.HairPartsPreviewImagePath);
        colorSelectUnitModel.SetCurrntColor(Model.ColorCodeDic[CharacterPartsType.Hair]);

        hairColorSelectUnit.SetModel(colorSelectUnitModel);

        await hairColorSelectUnit.ShowAsync();
    }

    private void SetCurrentDropdownValues()
    {
        SetCurrentRaceDropdownValue();
        SetCurrentHairDropdownValue();
    }

    private void SetCurrentRaceDropdownValue()
    {
        if (raceDropDown == null || Model.SelectableRaces == null)
            return;

        for (int i = 0; i < Model.SelectableRaces.Length; i++)
        {
            if (Model.SelectableRaces[i] == Model.SelectRace)
            {
                raceDropDown.value = i;
                return;
            }
        }
    }

    private void SetCurrentHairDropdownValue()
    {
        if (hairDropDown == null || Model.SelectableHairDatas == null)
            return;

        if (Model.SelectHairData == null)
        {
            hairDropDown.value = 0;
            return;
        }

        for (int i = 1; i < Model.SelectableHairDatas.Length; i++)
        {
            if (Model.SelectableHairDatas[i].Id == Model.SelectHairData.Id)
            {
                hairDropDown.value = i;
                return;
            }
        }
    }

    private void OnRaceDropdownChanged(int index)
    {
        if (Model.SelectableRaces == null || index < 0 || index >= Model.SelectableRaces.Length)
            return;

        var selectedRace = Model.SelectableRaces[index];
        Model.OnSelectRace?.Invoke(selectedRace);
    }

    private void OnHairDropdownChanged(int index)
    {
        if (Model.SelectableHairDatas == null || index < 0 || index >= Model.SelectableHairDatas.Length)
            return;

        var selectedHairData = Model.SelectableHairDatas[index];
        Model.OnSelectHair?.Invoke(selectedHairData);
    }

    private void OnHairColorChanged(string hex)
    {
        Model.OnSelectHairColor?.Invoke(hex);
    }

    private void OnShowHelmetToggleChanged(bool value)
    {
        Model.OnShowHelmet?.Invoke(value);
    }

    private void OnShowEquipmentToggleChanged(bool value)
    {
        Model.OnShowEquipments?.Invoke(value);
    }

    private void OnCompleteCustomize()
    {
        Model.OnCompleteCustomize?.Invoke();
    }
}
