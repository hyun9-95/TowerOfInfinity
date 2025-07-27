using UnityEngine;

public class TownSceneManager : BackgroundSceneManager<TownSceneManager>
{
    #region Property
    public CharacterUnit PlayerCharacter => playerCharacter;
    #endregion

    [Header("BattleFlow 이동 포탈")]
    [SerializeField]
    private Portal battlePortal;

    private CharacterUnit playerCharacter;

    public Portal GetBattlePortal()
    {
        return battlePortal;
    }

    public void SetPlayerCharacter(CharacterUnit leaderCharacter)
    {
        this.playerCharacter = leaderCharacter;
    }
}
