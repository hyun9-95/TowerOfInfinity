using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum CharacterDefine
{
	None = 0,
	CHAR_THIEF = 10000,
	CHAR_ARCHER = 10001,
	CHAR_BAT = 11000,

}