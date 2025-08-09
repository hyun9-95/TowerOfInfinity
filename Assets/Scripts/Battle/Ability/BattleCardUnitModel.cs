using UnityEngine;

public class BattleCardUnitModel : IBaseUnitModel
{
    #region Property
    public BattleCardTier Tier { get; set; }
    public string IconPath { get; set; }
    public string NameText { get; set; }
    public string DescriptionText { get; set; }
    #endregion

    #region Value

    #endregion

    #region Function
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
