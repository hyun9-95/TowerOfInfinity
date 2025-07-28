using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum LocalizationDefine
{
	None = 0,
	GAME_TITLE = 70000,

}