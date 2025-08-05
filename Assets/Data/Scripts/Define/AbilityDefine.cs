using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AbilityDefine
{
	None = 0,
	AB_ATTACK_SLASH_PROJECTILE = 50000,
	AB_ATTACK_ARROW = 50001,
	AB_ATTACK_STING = 50002,

}