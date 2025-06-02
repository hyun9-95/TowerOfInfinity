using Cysharp.Threading.Tasks;

public class WeaponProcessor
{
    public void Process(CharacterUnitModel model)
    {
        if (model.PendingWeaponCount == 0)
            return;

        while (model.PendingWeaponCount > 0)
        {
            var weapon = model.DequeuePendingWeapon();

            if (weapon == null)
                continue;

            weapon.Activate().Forget();

            Logger.BattleLog($"Weapon Activated! {weapon.GetType()} ({model.CharacterDefine})");
        }
    }

    public void Cancel(CharacterUnitModel model)
    {
        var weapons = model.GetAllWeapons();

        if (weapons == null)
            return;

        foreach (var weapon in weapons)
            weapon.Cancel();
    }
}
