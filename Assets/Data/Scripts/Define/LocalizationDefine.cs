using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum LocalizationDefine
{
	None = 0,
	GAME_TITLE = 70000,
	GAME_LOADING_DATA = 70001,
	GAME_LOADING_RESOURCES = 70002,
	GAME_LOADING_USER = 70003,

}