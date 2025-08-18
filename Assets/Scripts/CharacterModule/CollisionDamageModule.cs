using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Module/Collision Damage")]
public class CollisionDamageModule : ScriptableCharacterModule
{
    private DataBattleEvent defaultDamageEventData;

    public override ModuleType GetModuleType()
    {
        return ModuleType.CollisionDamage;
    }

    public override IModuleModel CreateModuleModel()
    {
        return new CollisionDamageModuleModel();
    }

    public override void OnEventTriggerEnter2D(Collider2D collider, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
        {
            if (IsTriggerTarget(collider))
            {
                var targetModel = BattleSceneManager.GetAliveCharModel(collider);

                if (targetModel != null)
                    moduleModel.CollisionEnterTargetTimer[collider.gameObject] = new CollisionDamageInfo(FloatDefine.DEFAULT_COLLISION_DAMAGE_INTERVAL, targetModel);
            }
        }
    }

    public override void OnEventTriggerStay2D(Collider2D collider, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
        {
            if (!moduleModel.CollisionEnterTargetTimer.TryGetValue(collider.gameObject, out var info))
                return;

            info.timer += Time.fixedDeltaTime;

            if (info.timer >= FloatDefine.DEFAULT_COLLISION_DAMAGE_INTERVAL)
            {
                info.timer = 0f;
                SendCollisionDamage(info, model);
            }
        }
    }

    public override void OnEventTriggerExit2D(Collider2D collider, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
            moduleModel.CollisionEnterTargetTimer.Remove(collider.gameObject);
    }

    private void SendCollisionDamage(CollisionDamageInfo info, CharacterUnitModel owner)
    {
        if (defaultDamageEventData.IsNullOrEmpty())
            defaultDamageEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)BattleEventDefine.BE_DAMAGE_ONE_SHOT_ATTACK);

        BattleEventModel eventModel = new (owner, info.targetModel, defaultDamageEventData, owner.Level);
        info.targetModel.BattleEventProcessor.ReceiveBattleEvent(eventModel);
    }

    private bool IsTriggerTarget(Collider2D collider)
    {
        return collider.gameObject.CheckLayer(LayerFlag.Character)
            && collider.gameObject.CompareTag(StringDefine.BATTLE_TAG_ALLY);
    }
}
