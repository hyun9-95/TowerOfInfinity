using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Module Group")]
public class ScriptableCharacterModuleGroup : ScriptableObject
{
    public IReadOnlyList<ScriptableCharacterModule> ModuleList => moduleList;

    [SerializeField]
    private List<ScriptableCharacterModule> moduleList = new List<ScriptableCharacterModule>();

    public Dictionary<ModuleType, IModuleModel> CreateModuleModelDic()
    {
        var moduleModelDic = new Dictionary<ModuleType, IModuleModel>();

        foreach (var module in moduleList)
        {
            var moduleModel = module.CreateModuleModel();

            if (moduleModel != null)
                moduleModelDic.Add(module.GetModuleType(), moduleModel);
        }

        return moduleModelDic;
    }

    public void OnEventUpdate(CharacterUnitModel model, Dictionary<ModuleType, IModuleModel> moduleModelDic)
    {
        foreach (var module in moduleList)
        {
            if (moduleModelDic.TryGetValue(module.GetModuleType(), out var moduleModel))
                module.OnEventUpdate(model, moduleModel);
        }
    }

    public void OnEventCharacterDeactivate(CharacterUnitModel model, Dictionary<ModuleType, IModuleModel> moduleModelDic)
    {
        foreach (var module in moduleList)
        {
            if (moduleModelDic.TryGetValue(module.GetModuleType(), out var moduleModel))
                module.OnCharacterDeactivate(model, moduleModel);
        }
    }

    public void OnEventTriggerEnter2D(Collider2D collision, CharacterUnitModel model, Dictionary<ModuleType, IModuleModel> moduleModelDic)
    {
        foreach (var module in moduleList)
        {
            if (moduleModelDic.TryGetValue(module.GetModuleType(), out var moduleModel))
                module.OnEventTriggerEnter2D(collision, model, moduleModel);
        }
    }

    public void OnEventTriggerStay2D(Collider2D collision, CharacterUnitModel model, Dictionary<ModuleType, IModuleModel> moduleModelDic)
    {
        foreach (var module in moduleList)
        {
            if (moduleModelDic.TryGetValue(module.GetModuleType(), out var moduleModel))
                module.OnEventTriggerStay2D(collision, model, moduleModel);
        }
    }

    public void OnEventTriggerExit2D(Collider2D collision, CharacterUnitModel model, Dictionary<ModuleType, IModuleModel> moduleModelDic)
    {
        foreach (var module in moduleList)
        {
            if (moduleModelDic.TryGetValue(module.GetModuleType(), out var moduleModel))
                module.OnEventTriggerExit2D(collision, model, moduleModel);
        }
    }
}
