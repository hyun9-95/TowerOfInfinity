using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum DungeonDefine
{
	None = 0,
	DUNGEON_ATLANTIS = 40000,

}