using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterModule/CharacterUIModule")]
public class CharacterUIModule : ScriptableCharacterModule
{
    public override ModuleType GetModuleType()
    {
        return ModuleType.CharacterUI;
    }

    public override IModuleModel CreateModuleModel()
    {
        return new CharacterUIModuleModel();
    }

    public override void OnEventUpdate(CharacterUnitModel model, IModuleModel IModuleModel)
    {
        base.OnEventUpdate(model, IModuleModel);
    }
}
