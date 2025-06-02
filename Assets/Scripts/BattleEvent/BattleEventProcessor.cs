using UnityEngine;

public class BattleEventProcessor
{
    public void Process(CharacterUnitModel model)
    {
        while (model.BattleEventCount > 0)
        {
            var battleEvent = model.DequeueBattleEvent();

            if (battleEvent == null)
                continue;

            if (battleEvent.IsValid)
                battleEvent.Process();

            battleEvent.ReturnToPool();
        }
    }
}
