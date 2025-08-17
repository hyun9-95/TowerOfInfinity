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
        CastAsync().Forget();
    }

    private async UniTask CastAsync()
    {
        BattleEventTriggerModel battleEventTriggerModel = Model.CreateTriggerModel();

        if (battleEventTriggerModel == null)
            return;

        RemainCoolTime = Model.CoolTime;

        var castEffectPath = Model.CastEffectPath;
        
        if (!string.IsNullOrEmpty(castEffectPath))
        {
            var castEffect = await ObjectPoolManager.Instance.SpawnPoolableMono<BattleEffect>
                (Model.CastEffectPath, Model.Owner.Transform.position, Quaternion.identity);
            
            if (castEffect.IsFollowTarget)
            {
                var model = new BattleEffectModel();
                model.SetFollowTarget(Model.Owner.Transform);

                castEffect.SetModel(model);
            }
            else
            {
                castEffect.transform.localPosition += castEffect.LocalPosOffset;
            }

            if (castEffect != null)
                await castEffect.ShowAwaitLifeTime();
        }

        BattleEventTrigger battleSkillTrigger = BattleEventTriggerFactory.Create(battleEventTriggerModel.TriggerType);
        battleSkillTrigger.SetModel(battleEventTriggerModel);
        battleSkillTrigger.Process().Forget();
    }
    #endregion

}
