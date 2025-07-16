using UnityEngine;

public class LobbySceneManager : BackgroundSceneManager<LobbySceneManager>
{
    [Header("BattleFlow 이동 포탈")]
    [SerializeField]
    private Portal battlePortal;

    public Portal GetBattlePortal()
    {
        return battlePortal;
    }
}
