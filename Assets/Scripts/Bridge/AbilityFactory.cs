using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public static class AbilityFactory
{
    private static Dictionary<AbilityDefine, AbilityBalance> abilityBalanceDic = new Dictionary<AbilityDefine, AbilityBalance>();

    public static async UniTask InitializeAbilityBalanceDic()
    {
        foreach (AbilityDefine ability in Enum.GetValues(typeof(AbilityDefine)))
        {
            string abilityName = ability.ToString();
            var abilityBalance = await AddressableManager.Instance.LoadScriptableObject<AbilityBalance>(abilityName);

            if (abilityBalance)
                abilityBalanceDic[ability] = abilityBalance;
        }
    }

    public static T Create<T>(int dataId, CharacterUnitModel owner) where T : Ability, new()
    {
        var model = GetAbilityModel(dataId, owner);

        if (model == null)
            return null;

        T abliity = new T();
        abliity.SetModel(model);

        return abliity;
    }

    private static AbilityModel GetAbilityModel(int dataId, CharacterUnitModel owner)
    {
        DataAbility abilityData = DataManager.Instance.GetDataById<DataAbility>(dataId);
        AbilityDefine abilityDefine = (AbilityDefine)abilityData.Id;

        if (abilityData.IsNull)
            return null;

        if (!abilityBalanceDic.TryGetValue(abilityDefine, out AbilityBalance abilityBalance))
            return null;

        AbilityModel abilityModel = new AbilityModel();
        abilityModel.SetByAbilityData(abilityData, owner, abilityBalance);

        return abilityModel;
    }
}
