using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterModule/CollisionDamage")]
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

    public override void OnEventTriggerEnter2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
        {
            if (IsTriggerTarget(collision))
            {
                var targetModel = BattleSceneManager.Instance.GetCharacterModel(collision.gameObject.GetInstanceID());

                if (targetModel != null)
                    moduleModel.CollisionEnterTargetTimer[collision.gameObject] = new CollisionDamageInfo(FloatDefine.DEFAULT_COLLISION_DAMAGE_INTERVAL, targetModel);
            }
        }
    }

    public override void OnEventTriggerStay2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
        {
            if (!moduleModel.CollisionEnterTargetTimer.TryGetValue(collision.gameObject, out var info))
                return;

            info.timer += Time.fixedDeltaTime;

            if (info.timer >= FloatDefine.DEFAULT_COLLISION_DAMAGE_INTERVAL)
            {
                info.timer = 0f;
                SendCollisionDamage(info, model);
            }
        }
    }

    public override void OnEventTriggerExit2D(Collider2D collision, CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (IModuleModel is CollisionDamageModuleModel moduleModel)
            moduleModel.CollisionEnterTargetTimer.Remove(collision.gameObject);
    }

    private void SendCollisionDamage(CollisionDamageInfo info, CharacterUnitModel owner)
    {
        if (defaultDamageEventData.IsNull)
            defaultDamageEventData = DataManager.Instance.GetDataById<DataBattleEvent>((int)BattleEventDefine.BE_DAMAGE_ONE_SHOT_ATTACK);

        var eventModel = new BattleEventModel();
        eventModel.Initialize(owner, info.targetModel, defaultDamageEventData, owner.Level);
        info.targetModel.EventProcessor.ReceiveBattleEvent(eventModel);
    }

    private bool IsTriggerTarget(Collider2D collision)
    {
        return collision.gameObject.CheckLayer(LayerFlag.Character)
            && collision.gameObject.CompareTag(StringDefine.BATTLE_TAG_ALLY);
    }
}
