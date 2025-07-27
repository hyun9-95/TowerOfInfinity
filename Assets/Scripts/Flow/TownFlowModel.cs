using UnityEngine;

public class TownFlowModel : IBaseFlowModel
{
    public SceneDefine LobbySceneDefine { get; private set; }

    public void SetLobbySceneDefine(SceneDefine define)
    {
        LobbySceneDefine = define;
    }
}
