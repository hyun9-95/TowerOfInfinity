using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum CharacterDefine
{
	None = 0,
	CHAR_THIEF = 10000,
	CHAR_ARCHER = 10001,
	CHAR_GUARD = 10002,
	CHAR_BAT = 11000,
	CHAR_GREEN_OGRE = 11001,
	CHAR_RED_DRAGON = 11002,

}