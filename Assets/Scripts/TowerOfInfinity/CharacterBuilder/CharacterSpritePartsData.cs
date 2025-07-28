using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddressableSpriteData", menuName = "TowerOfInfinity/Addressable Sprite Data")]
public class CharacterSpritePartsData : ScriptableObject
{
    [System.Serializable]
    public class PartData
    {
        public string PartName;
        public string Address;
    }

    [System.Serializable]
    public class LayerEntry
    {
        public string LayerName;
        public List<PartData> Parts;
    }

    public List<LayerEntry> LayerEntries = new List<LayerEntry>();
}