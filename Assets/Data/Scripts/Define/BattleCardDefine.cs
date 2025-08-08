using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleCardDefine
{
	None = 0,
	BATTLE_CARD_VENOM_BALL = 100000,

}