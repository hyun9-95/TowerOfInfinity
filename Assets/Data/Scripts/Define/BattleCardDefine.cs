using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleCardDefine
{
	None = 0,
	BATTLE_CARD_AB_VENOM_BALL = 100000,
	BATTLE_CARD_AB_BOULDER_SMASH = 100001,
	BATTLE_CARD_AB_SLASH_PROJECTILE = 100002,
	BATTLE_CARD_AB_MAGIC_MISSILE = 100003,
	BATTLE_CARD_EXP_GAIN_RANGE_UP = 100004,

}