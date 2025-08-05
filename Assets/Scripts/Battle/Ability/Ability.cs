#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Ability
{
    #region Property
    public AbilityModel Model { get; private set; }
    public float RemainCoolTime { get; private set; }
    public CastingType CastingType => Model.AbilityData.CastingType;
    public int DataID => Model.AbilityData.Id;
    public bool IsCastable => RemainCoolTime <= 0;
    #endregion

    #region Value
    #endregion

    #region Function
    public void Initialize(AbilityModel model)
    {
        Model = model;
        RemainCoolTime = model.CoolTime;
    }

    public void ReduceCoolTime()
    {
        RemainCoolTime -= Time.deltaTime;
    }

    public void SetRemainCoolTime(float value)
    {
        RemainCoolTime = value;
    }

    public async UniTask DelayCast(float delay)
    {
        // 딜레이 도중 캐스트 불가능하도록 미리 쿨타임 세팅
        RemainCoolTime = Model.CoolTime + delay;

        await UniTaskUtils.DelaySeconds(delay);

        Cast();
    }

    public void Cast()
    {
        BattleEventTriggerModel battleEventTriggerModel = Model.CreateTriggerModel();

        if (battleEventTriggerModel == null)
            return;

        BattleEventTrigger battleSkillTrigger = BattleEventTriggerFactory.Create(battleEventTriggerModel.TriggerType);
        battleSkillTrigger.SetModel(battleEventTriggerModel);
        battleSkillTrigger.Process().Forget();

        RemainCoolTime = Model.CoolTime;
    }
    #endregion

}
