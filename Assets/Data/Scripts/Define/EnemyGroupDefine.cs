using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum EnemyGroupDefine
{
	None = 0,
	ENEMY_GROUP_ATLANTIS = 30000,

}