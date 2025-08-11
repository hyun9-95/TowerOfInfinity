using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleEventDefine
{
	None = 0,
	BE_DAMAGE_ONE_SHOT_ATTACK = 20000,
	BE_DAMAGE_COLLISION = 20001,
	BE_POSION_VENOM_BALL = 20002,
	BE_DAMAGE_BOULDER_SMASH = 20003,
	BE_DAMAGE_SWORD_WAVE = 20004,
	BE_DAMAGE_MAGIC_MISSILE = 20005,
	BE_DAMAGE_ELECTRIC_CIRCLE = 20006,

}