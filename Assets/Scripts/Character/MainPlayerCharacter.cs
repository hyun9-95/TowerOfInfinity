using UnityEngine;

public class MainPlayerCharacter : MonoBehaviour
{
    #region Property
    public CharacterSpriteLibraryBuilder LibraryBuilder => libraryBuilder;

    public CharacterUnit CharacterUnit => characterUnit;
    #endregion

    #region Value
    [SerializeField]
    private CharacterSpriteLibraryBuilder libraryBuilder;

    [SerializeField]
    private CharacterUnit characterUnit;
    #endregion
}
