using System;
using System.Collections.Generic;

public class BattleCardSelectViewModel : IBaseViewModel
{
    #region Property
    public IReadOnlyList<BattleCardUnitModel> CardUnitModelList => cardUnitModels;

    public Action OnCompleteSelect { get; private set; }

    public Action<int> OnClickBattleCard { get; private set; }

    public Action<DataBattleCard> OnSelectBattleCard { get; private set; }
    #endregion

    #region Value
    private List<BattleCardUnitModel> cardUnitModels;
    #endregion

    #region Function
    public void SetBattleCardUnitModels(DataBattleCard[] cards)
    {
        cardUnitModels = CreateBattleCardUnitModelList(cards);
    }

    public void SetOnSelectBattleCard(Action<DataBattleCard> onSelectCard)
    {
        OnSelectBattleCard = onSelectCard;
    }

    public void SetOnClickBattleCard(Action<int> onClickBattleCard)
    {
        OnClickBattleCard = onClickBattleCard;
    }

    private List<BattleCardUnitModel> CreateBattleCardUnitModelList(DataBattleCard[] cards)
    {
        if (cards == null || cards.Length == 0)
            return null;

        var cardUnitModelList = new List<BattleCardUnitModel>();
        var abilityContainer = DataManager.Instance.GetDataContainer<DataAbility>();

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            var model = new BattleCardUnitModel();
            model.SetCardData(card);
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

            cardUnitModelList.Add(model);
        }

        return cardUnitModelList;
    }

    public void SetOnCompleteSelect(Action onCompleteSelect)
    {
        OnCompleteSelect = onCompleteSelect;
    }
    #endregion
}
