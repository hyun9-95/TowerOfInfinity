using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AbilityDefine
{
	None = 0,
	WEAPON_SLASH = 50000,
	WEAPON_SLASH_PROJECTILE = 50001,
	WEAPON_ARROW = 50002,
	WEAPON_STING = 50003,

}