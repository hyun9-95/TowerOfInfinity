using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AbilityDefine
{
	None = 0,
	AB_WEAPON_SLASH_PROJECTILE = 50000,
	AB_WEAPON_ARROW = 50001,
	AB_WEAPON_STING = 50002,
	AB_ACTIVE_VENOM_BALL = 51000,

}