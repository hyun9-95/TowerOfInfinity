using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum EquipmentDefine
{
	None = 0,
	EQUIPMENT_ARMOR_THIEF_TUNIC = 80000,
	EQUIPMENT_ARMOR_THIEF_HOOD = 80001,
	EQUIPMENT_BRACERS_THIEF_TUNIC = 80002,
	EQUIPMENT_WEAPON_SHORT_DAGGER = 80003,

}