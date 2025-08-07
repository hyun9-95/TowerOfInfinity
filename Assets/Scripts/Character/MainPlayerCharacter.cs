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

    [SerializeField]
    private MainCharacterInput mainCharacterInput;
    #endregion

    public async UniTask UpdateMainCharacter(MainCharacterInfo mainCharacterInfo, CharacterSetUpType setUpType)
    {
        if (characterUnit.Model == null)
            characterUnit.SetModel(new CharacterUnitModel());

        CharacterFactory.Instance.SetCharacterUnitModel(
            characterUnit.Model, TeamTag.Ally, CharacterType.Main, setUpType, mainCharacterInfo);

        var model = characterUnit.Model;

        // 장비 장착
        foreach (var equipment in mainCharacterInfo.EquippedEquipments.Values)
            model.EquipEquipment(equipment);

        // 조작 활성화
        mainCharacterInput.Initialize(model);

        // 외형 업데이트
        await UpdateSpriteLibraryAsset(mainCharacterInfo.PartsInfo);

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

    public async UniTask UpdateSpriteLibraryAsset(IEnumerable<DataCharacterParts> changeParts)
    {
        var spriteLibrary = characterUnit.SpriteLibrary;

        if (spriteLibrary != null && spriteLibrary.spriteLibraryAsset != null)
        {
            // 기존 스프라이트 라이브러리 에셋은 제거
            DestroyImmediate(spriteLibrary.spriteLibraryAsset);
        }

        var spriteLibraryAsset = await LibraryBuilder.UpdateParts(changeParts);
        characterUnit.SetSpriteLibraryAsset(spriteLibraryAsset);
    }
}
