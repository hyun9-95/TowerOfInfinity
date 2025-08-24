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
    private Toggle showHelmentToggle;

    [SerializeField]
    private Toggle showEquipmentToggle;

    [SerializeField]
    private Button completeButton;

    [Header("Color Customization")]
    [SerializeField]
    private GameObject colorCustomizationPanel;

    [SerializeField] 
    private Button[] partsEditButtons;

    [SerializeField]
    private Slider redSlider;

    [SerializeField]
    private Slider greenSlider;

    [SerializeField]
    private Slider blueSlider;

    [SerializeField]
    private Image colorPreview;

    [SerializeField]
    private Slider hueSlider;

    [SerializeField]
    private Slider saturationSlider;

    [SerializeField]
    private Slider valueSlider;

    [SerializeField]
    private Button resetColorButton;

    private void Start()
    {
        SetupEventListeners();
    }

    public override async UniTask ShowAsync()
    {
        SetupDropdownOptions();
        SetupToggleValues();
    }

    private void SetupEventListeners()
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

        SetupColorCustomizationListeners();
    }

    private void SetupDropdownOptions()
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

    private void SetupToggleValues()
    {
        if (showHelmentToggle != null)
            showHelmentToggle.isOn = Model.IsShowHelmet;

        if (showEquipmentToggle != null)
            showEquipmentToggle.isOn = Model.IsShowEquipments;
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

        if (Model.SelectHairInfo == null)
        {
            hairDropDown.value = 0;
            return;
        }

        var currentHairData = Model.SelectHairInfo.GetPartsData();
        if (currentHairData != null)
        {
            for (int i = 1; i < Model.SelectableHairDatas.Length; i++)
            {
                if (Model.SelectableHairDatas[i] != null && Model.SelectableHairDatas[i].Id == currentHairData.Id)
                {
                    hairDropDown.value = i;
                    return;
                }
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

    private void SetupColorCustomizationListeners()
    {
        if (partsEditButtons != null)
        {
            for (int i = 0; i < partsEditButtons.Length; i++)
            {
                var partsType = (CharacterPartsType)i;
                partsEditButtons[i]?.onClick.AddListener(() => OnSelectPartsForEdit(partsType));
            }
        }

        redSlider?.onValueChanged.AddListener(OnColorSliderChanged);
        greenSlider?.onValueChanged.AddListener(OnColorSliderChanged);
        blueSlider?.onValueChanged.AddListener(OnColorSliderChanged);

        hueSlider?.onValueChanged.AddListener(OnHSVSliderChanged);
        saturationSlider?.onValueChanged.AddListener(OnHSVSliderChanged);
        valueSlider?.onValueChanged.AddListener(OnHSVSliderChanged);

        resetColorButton?.onClick.AddListener(OnResetColor);
    }

    private void OnSelectPartsForEdit(CharacterPartsType partsType)
    {
        Model.OnSelectPartsForEdit?.Invoke(partsType);
        UpdateColorCustomizationPanel();
    }

    private void OnColorSliderChanged(float value)
    {
        if (redSlider == null || greenSlider == null || blueSlider == null)
            return;

        var color = new Color(redSlider.value / 255f, greenSlider.value / 255f, blueSlider.value / 255f);
        UpdateColorPreview(color);
        Model.OnChangeColor?.Invoke(color);
    }

    private void OnHSVSliderChanged(float value)
    {
        if (hueSlider == null || saturationSlider == null || valueSlider == null)
            return;

        var hsv = new Vector3(hueSlider.value, saturationSlider.value, valueSlider.value);
        Model.OnChangeHSV?.Invoke(hsv);
    }

    private void OnResetColor()
    {
        if (redSlider != null) redSlider.value = 255f;
        if (greenSlider != null) greenSlider.value = 255f;
        if (blueSlider != null) blueSlider.value = 255f;

        if (hueSlider != null) hueSlider.value = 0f;
        if (saturationSlider != null) saturationSlider.value = 0f;
        if (valueSlider != null) valueSlider.value = 0f;

        UpdateColorPreview(Color.white);
        Model.OnChangeColor?.Invoke(Color.white);
        Model.OnChangeHSV?.Invoke(Vector3.zero);
    }

    private void UpdateColorCustomizationPanel()
    {
        if (colorCustomizationPanel == null)
            return;

        bool hasEditingParts = Model.CurrentEditingPartsInfo != null;
        colorCustomizationPanel.SetActive(hasEditingParts);

        if (hasEditingParts)
        {
            UpdateColorSliders();
            UpdateHSVSliders();
        }
    }

    private void UpdateColorSliders()
    {
        var currentColor = Model.GetCurrentPartsColor();
        
        if (redSlider != null) redSlider.value = currentColor.r * 255f;
        if (greenSlider != null) greenSlider.value = currentColor.g * 255f;
        if (blueSlider != null) blueSlider.value = currentColor.b * 255f;

        UpdateColorPreview(currentColor);
    }

    private void UpdateHSVSliders()
    {
        var currentHSV = Model.GetCurrentPartsHSV();

        if (hueSlider != null) hueSlider.value = currentHSV.x;
        if (saturationSlider != null) saturationSlider.value = currentHSV.y;
        if (valueSlider != null) valueSlider.value = currentHSV.z;
    }

    private void UpdateColorPreview(Color color)
    {
        if (colorPreview != null)
            colorPreview.color = color;
    }
}
