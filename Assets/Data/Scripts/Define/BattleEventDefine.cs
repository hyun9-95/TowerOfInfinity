using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleEventDefine
{
	None = 0,
	BE_SLASH_RANGE = 20000,
	BE_SLASH_PROJECTILE = 20001,
	BE_ARROW_PROJECTILE = 20002,
	BE_STING = 20003,

}