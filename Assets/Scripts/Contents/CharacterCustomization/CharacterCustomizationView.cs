#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterCustomizationView : BaseView
{
    public CharacterCustomizationViewModel Model => GetModel<CharacterCustomizationViewModel>();

    [SerializeField]
    private CharacterSpriteLibraryBuilder characterSpriteLibraryBuilder;

    [SerializeField]
    private TMP_Dropdown[] partDropdown;


    private DataContainer<DataCharacterParts> partsContainer;
    private bool isInitialized = false;

    public override async UniTask ShowAsync()
    {
        LoadCharacterSpritePartsData();
        InitializeDropdowns();
        UpdateUI();
        isInitialized = true;
    }

    private void LoadCharacterSpritePartsData()
    {
        partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
    }

    private void InitializeDropdowns()
    {
        if (partsContainer == null || partDropdown == null)
            return;

        var partsEnumArray = Enum.GetValues(typeof(CharacterPartsType)).Cast<CharacterPartsType>().ToArray();
        
        for (int i = 0; i < partDropdown.Length && i < partsEnumArray.Length; i++)
        {
            var dropdown = partDropdown[i];
            var partType = partsEnumArray[i];
            
            SetupDropdownOptions(dropdown, partType, i);
        }
    }

    private void SetupDropdownOptions(TMP_Dropdown dropdown, CharacterPartsType partType, int partIndex)
    {
        dropdown.ClearOptions();
        
        var options = new List<string> { "None" };

        var partsOfType = partsContainer.FindAll(p => !p.IsNull && p.PartsType == partType);
            
        foreach (var part in partsOfType)
        {
            var partName = GetPartNameFromPath(part.PartsPath);
            if (!string.IsNullOrEmpty(partName))
                options.Add(partName);
        }
        
        dropdown.AddOptions(options);
        
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) => OnDropdownValueChanged(partIndex, value));
        
        var currentPart = Model.Parts[partIndex];
        if (!string.IsNullOrEmpty(currentPart))
        {
            var optionIndex = options.IndexOf(currentPart);
            if (optionIndex >= 0)
                dropdown.value = optionIndex;
        }
        else
        {
            if (partType == CharacterPartsType.Body || partType == CharacterPartsType.Head)
            {
                if (options.Count > 1)
                {
                    dropdown.value = 1;
                    Model.ChangePart(partIndex, options[1]);
                }
            }
        }
    }
    
    private string GetPartNameFromPath(string partsPath)
    {
        if (string.IsNullOrEmpty(partsPath))
            return string.Empty;
            
        var lastSlashIndex = partsPath.LastIndexOf('/');
        if (lastSlashIndex >= 0 && lastSlashIndex < partsPath.Length - 1)
            return partsPath.Substring(lastSlashIndex + 1);
            
        return partsPath;
    }

    private async void OnDropdownValueChanged(int partIndex, int optionIndex)
    {
        if (!isInitialized)
            return;
            
        var dropdown = partDropdown[partIndex];
        var selectedOption = dropdown.options[optionIndex].text;
        
        // "None" 선택시 빈 문자열로 처리
        var partValue = selectedOption == "None" ? "" : selectedOption;
        
        await OnChangePart(partIndex, partValue);
    }

    public void UpdateUI()
    {
        if (partsContainer == null)
            return;
            
        for (int i = 0; i < partDropdown.Length && i < Model.Parts.Count; i++)
        {
            var currentPart = Model.Parts[i];
            var dropdown = partDropdown[i];
            
            if (string.IsNullOrEmpty(currentPart))
            {
                dropdown.value = 0;
            }
            else
            {
                var optionIndex = dropdown.options.FindIndex(option => option.text == currentPart);
                if (optionIndex >= 0)
                    dropdown.value = optionIndex;
            }
        }
    }

    public async UniTask OnChangePart(int index, string value)
    {
        Model.ChangePart(index, value);
        
        try
        {
            // 최소한 하나의 파츠라도 유효한지 확인
            if (HasValidParts())
            {
                var spriteLibraryAsset = await characterSpriteLibraryBuilder.Rebuild(Model.Parts.ToArray());
                if (spriteLibraryAsset != null)
                {
                    Model.SpriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to rebuild sprite library: {ex.Message}");
        }
    }

    private bool HasValidParts()
    {
        if (Model.Parts == null || partsContainer == null)
            return false;

        for (int i = 0; i < Model.Parts.Count; i++)
        {
            var partValue = Model.Parts[i];
            if (string.IsNullOrEmpty(partValue))
                continue;
                
            var partType = (CharacterPartsType)i;
            var part = partsContainer.Find(p => !p.IsNull &&
                    p.PartsType == partType &&
                    GetPartNameFromPath(p.PartsPath) == partValue);
                    
            if (!part.IsNull && !string.IsNullOrEmpty(part.PartsPath))
                return true;
        }
        
        return false;
    }

    private void OnSaveButtonClicked()
    {
    }
}
