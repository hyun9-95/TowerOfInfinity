using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum EquipmentDefine
{
	None = 0,
	EQUIPMENT_ARMOR_THIEF_TUNIC = 90000,
	EQUIPMENT_ARMOR_THIEF_HOOD = 90001,
	EQUIPMENT_BRACERS_THIEF_TUNIC = 90002,
	EQUIPMENT_WEAPON_SHORT_DAGGER = 90003,

}