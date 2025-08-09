using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleCardDrawer
{
	public struct CardDrawRate
	{
		public float startValue;
		public float endValue;
	}

	#region Property
	#endregion

	#region Value
	private Dictionary<BattleCardTier, CardDrawRate> drawRateByTierDic = new();
	private Dictionary<BattleCardTier, List<DataBattleCard>> cardByTierDic = new();
	private int maxLevel;
	private int drawCount;
	#endregion
	
	#region Function
	public void Initialize()
	{
		maxLevel = IntDefine.MAX_BATTLE_LEVEL;
		drawCount = IntDefine.BATTLE_CARD_DRAW_COUNT;

		drawRateByTierDic.Clear();
		cardByTierDic.Clear();

        var balanceContainer = DataManager.Instance.GetDataContainer<DataBalance>();

		var startTierRateData = balanceContainer.GetById((int)BalanceDefine.BALANCE_BATTLE_CARD_DRAW_TIER_PROB_START);
		var endTierRateData = balanceContainer.GetById((int)BalanceDefine.BALANCE_BATTLE_CARD_DRAW_TIER_PROB_END);

		foreach (BattleCardTier tier in Enum.GetValues(typeof(BattleCardTier)))
		{
			int index = (int)tier;

			var drawRate = new CardDrawRate();

			// 확률은 백분율
			drawRate.startValue = startTierRateData.Values[index] / 100;
			drawRate.endValue = endTierRateData.Values[index] / 100;

			drawRateByTierDic[tier] = drawRate;
		}

		var allCardDatas = DataManager.Instance.FindAllData<DataBattleCard>(x => !x.IsNull);

		foreach (var data in allCardDatas)
		{
			if (!cardByTierDic.ContainsKey(data.Tier))
                cardByTierDic[data.Tier] = new List<DataBattleCard>();

            cardByTierDic[data.Tier].Add(data);
        }
	}

	private BattleCardTier GetDrawTier(int battleLevel)
	{
        float progress = (float)battleLevel / (maxLevel - 1);
        float roll = UnityEngine.Random.value;
		float currentRate = 0;

        foreach (BattleCardTier tier in Enum.GetValues(typeof(BattleCardTier)))
        {
			var drawRate = drawRateByTierDic[tier];
			currentRate += Mathf.Lerp(drawRate.startValue, drawRate.endValue, progress);

			if (roll <= currentRate)
				return tier;
		}

		return BattleCardTier.Epic;
	}

    public DataBattleCard[] DrawBattleCards(int battleLevel)
    {
        var resultCards = new DataBattleCard[drawCount];
        var drawnCardsIds = new HashSet<int>();

        int drawnCount = 0;  // 실제 뽑은 카드 수

        while (drawnCount < resultCards.Length)
        {
            BattleCardTier drawTier = GetDrawTier(battleLevel);

            // 해당 티어에 카드가 없다면 Common
            if (!cardByTierDic.ContainsKey(drawTier))
                drawTier = BattleCardTier.Common;

            var cards = cardByTierDic[drawTier];
            var drawIndex = UnityEngine.Random.Range(0, cards.Count);
            var drawnCard = cards[drawIndex];

			// 중복카드 비허용
            if (drawnCardsIds.Contains(drawnCard.Id))
                continue;

            resultCards[drawnCount] = drawnCard;
            drawnCardsIds.Add(drawnCard.Id);
            drawnCount++;
        }

        return resultCards;
    }
    #endregion
}
