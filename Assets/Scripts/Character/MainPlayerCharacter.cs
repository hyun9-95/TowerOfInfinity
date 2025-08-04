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

    public async UniTask<bool> UpdateMainCharacter(MainCharacterInfo mainCharacterInfo)
    {
        int defaultWeaponDataId = mainCharacterInfo.DefaultWeaponDataId;
        int activeSkillDataId = mainCharacterInfo.ActiveSkillDataId;
        int passiveSkillDataId = mainCharacterInfo.PassiveSkillDataId;

        bool result = await CharacterFactory.Instance.SetCharacterUnitModel(characterUnit, TeamTag.Ally, CharacterType.Main,
            defaultWeaponDataId, activeSkillDataId, passiveSkillDataId);

        if (result)
        {
            mainCharacterInput.Initialize(characterUnit.Model);

            await UpdateSpriteLibraryAsset(mainCharacterInfo.PartsInfo);
        }
        
        return result;
    }

    public async UniTask UpdateSpriteLibraryAsset(MainCharacterPartsInfo mainCharacterPartsInfo)
    {
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
