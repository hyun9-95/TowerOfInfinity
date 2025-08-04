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
    private Button completeButton;

    private void Start()
    {
        SetupEventListeners();
    }

    public override async UniTask ShowAsync()
    {
        SetupDropdownOptions();
    }

    private void SetupEventListeners()
    {
        if (raceDropDown != null)
            raceDropDown.onValueChanged.AddListener(OnRaceDropdownChanged);

        if (hairDropDown != null)
            hairDropDown.onValueChanged.AddListener(OnHairDropdownChanged);

        if (completeButton != null)
            completeButton.onClick.AddListener(OnCompleteCustomize);
    }

    private void SetupDropdownOptions()
    {
        SetupRaceDropdownOptions();
        SetupHairDropdownOptions();
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
            string.IsNullOrEmpty(hairData.PartsPath) ? "None" : 
            hairData.PartsPath.Substring(hairData.PartsPath.LastIndexOf('/') + 1)).ToList();
        hairDropDown.AddOptions(hairOptions);
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

    private void OnCompleteCustomize()
    {
        Model.OnCompleteCustomize?.Invoke();
    }
}
