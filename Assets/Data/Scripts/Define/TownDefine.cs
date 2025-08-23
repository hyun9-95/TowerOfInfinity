using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum TownDefine
{
	None = 0,
	TOWN_RUINS = 110000,
	TOWN_CUSTOMIZATION = 110001,

}