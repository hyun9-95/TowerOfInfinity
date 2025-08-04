using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainPlayerCharacter : AddressableMono
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

    public async UniTask<bool> UpdateModel(MainCharacterInfo mainCharacterInfo)
    {
        int defaultWeaponDataId = mainCharacterInfo.DefaultWeaponDataId;
        int activeSkillDataId = mainCharacterInfo.ActiveSkillDataId;
        int passiveSkillDataId = mainCharacterInfo.PassiveSkillDataId;

        bool result = await CharacterFactory.Instance.SetCharacterUnitModel(characterUnit, TeamTag.Ally, CharacterType.Main,
            defaultWeaponDataId, activeSkillDataId, passiveSkillDataId);

        if (result)
        {
            var spriteLibrary = await LibraryBuilder.Rebuild(mainCharacterInfo.PartsInfo);
            characterUnit.SetSpriteLibraryAsset(spriteLibrary);

            mainCharacterInput.Initialize(characterUnit.Model);
        }
        
        return result;
    }
}
