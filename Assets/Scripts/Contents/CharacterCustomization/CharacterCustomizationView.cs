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

    [SerializeField]
    private Transform partsContainer;

    private CharacterSpritePartsData characterSpritePartsData;
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
        characterSpritePartsData = Resources.Load<CharacterSpritePartsData>(PathDefine.CHARACTER_PARTS_DATA);
    }

    private void InitializeDropdowns()
    {
        if (characterSpritePartsData == null || partDropdown == null)
            return;

        var partsEnumArray = Enum.GetValues(typeof(CharacterPartsName)).Cast<CharacterPartsName>().ToArray();
        
        for (int i = 0; i < partDropdown.Length && i < partsEnumArray.Length; i++)
        {
            var dropdown = partDropdown[i];
            var partName = partsEnumArray[i].ToString();
            
            // 해당 파트의 드롭다운 옵션 설정
            SetupDropdownOptions(dropdown, partName, i);
        }
    }

    private void SetupDropdownOptions(TMP_Dropdown dropdown, string partName, int partIndex)
    {
        dropdown.ClearOptions();
        
        var options = new List<string> { "None" };
        
        // CharacterSpritePartsData에서 해당 파트의 옵션들을 가져옴
        var layerEntry = characterSpritePartsData.LayerEntries.FirstOrDefault(entry => entry.LayerName == partName);
        if (layerEntry != null && layerEntry.Parts != null)
        {
            options.AddRange(layerEntry.Parts.Select(part => part.PartName));
        }
        
        dropdown.AddOptions(options);
        
        // 드롭다운 변경 이벤트 등록
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((value) => OnDropdownValueChanged(partIndex, value));
        
        // 현재 선택된 값으로 설정 또는 기본값 설정
        var currentPart = Model.Parts[partIndex];
        if (!string.IsNullOrEmpty(currentPart))
        {
            var optionIndex = options.IndexOf(currentPart);
            if (optionIndex >= 0)
            {
                dropdown.value = optionIndex;
            }
        }
        else
        {
            // 기본 파츠 설정 (Body와 Head는 기본적으로 필요)
            if (partName == "Body" || partName == "Head")
            {
                if (options.Count > 1) // "None" 외에 다른 옵션이 있는 경우
                {
                    dropdown.value = 1; // 첫 번째 실제 파츠 선택
                    Model.ChangePart(partIndex, options[1]);
                }
            }
        }
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
        if (characterSpritePartsData == null)
            return;
            
        // 드롭다운들의 현재 값을 Model의 Parts와 동기화
        for (int i = 0; i < partDropdown.Length && i < Model.Parts.Count; i++)
        {
            var currentPart = Model.Parts[i];
            var dropdown = partDropdown[i];
            
            if (string.IsNullOrEmpty(currentPart))
            {
                dropdown.value = 0; // "None" 선택
            }
            else
            {
                var optionIndex = dropdown.options.FindIndex(option => option.text == currentPart);
                if (optionIndex >= 0)
                {
                    dropdown.value = optionIndex;
                }
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
        if (Model.Parts == null || characterSpritePartsData == null)
            return false;

        for (int i = 0; i < Model.Parts.Count; i++)
        {
            var partValue = Model.Parts[i];
            if (!string.IsNullOrEmpty(partValue))
            {
                var partName = ((CharacterPartsName)i).ToString();
                
                // CharacterSpritePartsData에서 해당 파츠가 실제로 존재하는지 확인
                var layerEntry = characterSpritePartsData.LayerEntries.FirstOrDefault(entry => entry.LayerName == partName);
                if (layerEntry != null && layerEntry.Parts != null)
                {
                    var part = layerEntry.Parts.FirstOrDefault(p => p.PartName == partValue);
                    if (part != null && !string.IsNullOrEmpty(part.Address))
                    {
                        return true; // 최소한 하나의 유효한 파츠가 있음
                    }
                }
            }
        }
        
        return false; // 유효한 파츠가 없음
    }

    private void OnSaveButtonClicked()
    {
        var userCharacterAppearanceInfo = new UserCharacterAppearanceInfo();
        userCharacterAppearanceInfo.parts = Model.Parts.ToArray();
        // 데이터를 저장하는 로직 추가
        // 예: PlayerManager나 UserManager를 통해 저장
    }
}
