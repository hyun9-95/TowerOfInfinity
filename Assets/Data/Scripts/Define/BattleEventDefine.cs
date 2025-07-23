using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleEventDefine
{
	None = 0,
	BE_DAMAGE_ONE_SHOT_ATTACK = 20000,

}