using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AbilityDefine
{
	None = 0,
	AB_WEAPON_ARROW = 50000,
	AB_WEAPON_STING = 50001,
	AB_ACTIVE_VENOM_BALL = 51000,
	AB_ACTIVE_BOULDER_SMASH = 51001,
	AB_ACTIVE_SWORD_WAVE = 51002,
	AB_ACTIVE_MAGIC_MISSILE = 51003,
	AB_ACTIVE_ELECTRIC_CIRCLE = 51004,

}