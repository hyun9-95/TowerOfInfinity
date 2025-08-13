public static class AbilityFactory
{
    public static T Create<T>(int dataId, CharacterUnitModel owner) where T : Ability, new()
    {
        DataAbility abilityData = DataManager.Instance.GetDataById<DataAbility>(dataId);

        return Create<T>(abilityData, owner);
    }

    public static T Create<T>(DataAbility data, CharacterUnitModel owner) where T : Ability, new()
    {
        var model = GetAbilityModel(data, owner);

        if (model == null)
            return null;

        T abliity = new T();
        abliity.Initialize(model);

        return abliity;
    }

    private static AbilityModel GetAbilityModel(DataAbility abilityData, CharacterUnitModel owner)
    {
        AbilityDefine abilityDefine = (AbilityDefine)abilityData.Id;

        if (abilityData.IsNullOrEmpty())
            return null;

        AbilityModel abilityModel = new AbilityModel();
        abilityModel.SetByAbilityData(abilityData, owner);

        return abilityModel;
    }
}
