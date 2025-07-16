using UnityEngine;

public class BattleEventProcessor
{
    public CharacterUnitModel OwnerModel { get; private set; }

    public void Process(CharacterUnitModel model)
    {
        while (model.BattleEventCount > 0)
        {
            var battleEvent = model.DequeueBattleEvent();

            if (battleEvent == null)
                continue;

            if (battleEvent.IsValid)
                battleEvent.OnStart();

            battleEvent.ReturnToPool();
        }
    }
}
