using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public static class AbilityFactory
{
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

        AbilityModel abilityModel = new AbilityModel();
        abilityModel.SetByAbilityData(abilityData, owner);

        return abilityModel;
    }
}
