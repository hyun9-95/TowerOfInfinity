using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleEventDefine
{
	None = 0,
	BE_DAMAGE_ONE_SHOT_ATTACK = 20000,
	BE_DAMAGE_COLLISION = 20001,
	BE_POSION_VENOM_BALL = 20002,

}