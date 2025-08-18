using System;
using UnityEngine;

public class BattleCardUnitModel : IBaseUnitModel
{
    #region Property
    public DataBattleCard CardData { get; private set; }
    public BattleCardTier Tier { get; private set; }
    public string IconPath { get; private set; }
    public string NameText { get; private set; }
    public string DescriptionText { get; private set; }
    #endregion

    #region Value

    #endregion

    #region Function
    public void SetCardData(DataBattleCard card)
    {
        CardData = card;
    }

    public void SetTier(BattleCardTier tier)
    {
        Tier = tier;
    }

    public void SetNameText(string name)
	{
		NameText = name;
	}

    public void SetDescriptionText(string description)
    {
        DescriptionText = description;
    }

    public void SetIconPath(string iconPath)
    {
        IconPath = iconPath;
    }

    public string GetTierFramePath()
    {
        int tierInt = (int)Tier;

        return string.Format(PathDefine.CARD_TIER_FRAME_FORMAT, tierInt, Tier.ToString());
    }
	#endregion
}
