using System;
using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModels => cardUnitModels;

    public Action OnCompleteSelect { get; private set; }
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels = new List<BattleCardUnitModel>();
    #endregion

    #region Function
    public void SetBattleCards(DataBattleCard[] cards)
    {
        cardUnitModels.Clear();

        var abilityContainer = DataManager.Instance.GetDataContainer<DataAbility>();
       
        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            var model = new BattleCardUnitModel();
            model.SetTier(card.Tier);

            if (card.CardType == BattleCardType.GetAbility)
            {
                var abilityData = abilityContainer.GetById((int)card.Ability);
                model.SetIconPath(abilityData.IconPath);
            }
            else
            {
                model.SetIconPath(card.IconPath);
            }

            model.SetNameText(LocalizationManager.GetLocalization(card.Name));
            model.SetDescriptionText(LocalizationManager.GetLocalization(card.Desc));

            cardUnitModels.Add(model);
        }
    }

    public void SetOnCompleteSelect(Action onCompleteSelect)
    {
        OnCompleteSelect = onCompleteSelect;
    }
    #endregion
}
