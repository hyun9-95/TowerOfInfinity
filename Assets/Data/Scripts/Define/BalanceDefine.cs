using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BalanceDefine
{
	None = 0,
	BALANCE_BATTLE_START_EXP = 60000,
	BALANCE_BATTLE_EXP_GROWTH_RATE = 60001,
	BALANCE_BATTLE_CARD_DRAW_TIER_PROB_START = 60002,
	BALANCE_BATTLE_CARD_DRAW_TIER_PROB_END = 60003,

}