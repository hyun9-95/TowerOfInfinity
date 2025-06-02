using UnityEngine;

public class LobbyFlowModel : IBaseFlowModel
{
    public SceneDefine LobbySceneDefine { get; private set; }

    public void SetLobbySceneDefine(SceneDefine define)
    {
        LobbySceneDefine = define;
    }
}
