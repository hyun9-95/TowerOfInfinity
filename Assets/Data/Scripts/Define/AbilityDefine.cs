using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AbilityDefine
{
	None = 0,
	WEAPON_SLASH_PROJECTILE = 50000,
	WEAPON_ARROW = 50001,
	WEAPON_STING = 50002,

}