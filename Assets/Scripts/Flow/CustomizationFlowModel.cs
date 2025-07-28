using UnityEngine;
using UnityEngine.U2D.Animation;

public class CustomizationFlowModel : IBaseFlowModel
{
    public SceneDefine CustomizationSceneDefine { get; private set; }
    public UserCharacterAppearanceInfo UserCharacterInfo { get; private set; }

    public void SetUserCharacterInfo(UserCharacterAppearanceInfo characterInfo)
    {
        UserCharacterInfo = characterInfo;
    }
}