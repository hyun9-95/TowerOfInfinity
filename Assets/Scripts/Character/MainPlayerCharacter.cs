using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCharacter : MonoBehaviour
{
    #region Property
    public CharacterSpriteLibraryBuilder LibraryBuilder => libraryBuilder;

    public CharacterUnit CharacterUnit => characterUnit;
    #endregion

    #region Value
    [SerializeField]
    private CharacterUnit characterUnit;

    [SerializeField]
    private CharacterSpriteLibraryBuilder libraryBuilder;
    #endregion

    public async UniTask UpdateMainCharacter(MainCharacterInfo mainCharacterInfo)
    {
        if (characterUnit.Model == null)
            characterUnit.SetModel(new CharacterUnitModel());

        CharacterFactory.Instance.SetCharacterUnitModel(
            characterUnit.Model, TeamTag.Ally, CharacterType.Main, mainCharacterInfo);

        var model = characterUnit.Model;

        // 장비 장착
        foreach (var equipment in mainCharacterInfo.EquippedEquipments.Values)
            model.EquipEquipment(equipment);

        // 외형 업데이트
        await UpdateSpriteLibraryAsset(mainCharacterInfo.PartsInfo);

        // 인풋 정보 할당
        model.SetInputWrapper(InputManager.InputInfo);

        if (model.CharacterSetUpType != CharacterSetUpType.Battle)
        {
            if (characterUnit.TryGetComponent<BattleExpGainer>(out var battleExpGainer))
                DestroyImmediate(battleExpGainer);
        }
    }

    public async UniTask UpdateSpriteLibraryAsset(MainCharacterPartsInfo mainCharacterPartsInfo)
    {
        bool isChangedParts = LibraryBuilder.CheckPartsChange(mainCharacterPartsInfo);

        if (!isChangedParts)
            return;

        var spriteLibrary = characterUnit.SpriteLibrary;

        if (spriteLibrary != null && spriteLibrary.spriteLibraryAsset != null)
        {
            // 기존 스프라이트 라이브러리 에셋은 제거
            DestroyImmediate(spriteLibrary.spriteLibraryAsset);
        }

        var spriteLibraryAsset = await LibraryBuilder.CreateNewSpriteLibrary(mainCharacterPartsInfo);
        characterUnit.SetSpriteLibraryAsset(spriteLibraryAsset);
    }

    public async UniTask UpdateSpriteLibraryAsset(IEnumerable<CharacterPartsInfo> changeParts, IEnumerable<CharacterPartsType> removeTypes)
    {
        var spriteLibrary = characterUnit.SpriteLibrary;

        if (spriteLibrary != null && spriteLibrary.spriteLibraryAsset != null)
        {
            // 기존 스프라이트 라이브러리 에셋은 제거
            DestroyImmediate(spriteLibrary.spriteLibraryAsset);
        }

        var spriteLibraryAsset = await LibraryBuilder.UpdateParts(changeParts, removeTypes);
        characterUnit.SetSpriteLibraryAsset(spriteLibraryAsset);
    }
}
