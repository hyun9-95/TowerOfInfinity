using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Module/Character UI")]
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
