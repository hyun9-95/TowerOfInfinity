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
	AB_ACTIVE_SLASH_PROJECTILE = 51002,

}