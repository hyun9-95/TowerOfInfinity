using UnityEngine;

public abstract class ScriptableCharacterModule : ScriptableObject
{
    public abstract ModuleType GetModuleType();

    public virtual IModuleModel CreateModuleModel() { return null; }

    public virtual void OnEventUpdate(CharacterUnitModel model, IModuleModel IModuleModel) { }

    public virtual void OnCharacterDeactivate(CharacterUnitModel model, IModuleModel IModuleModel) { }

    public virtual void OnEventTriggerEnter2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel) { }
        
    public virtual void OnEventTriggerStay2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel) { }

    public virtual void OnEventTriggerExit2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel) { }
}
