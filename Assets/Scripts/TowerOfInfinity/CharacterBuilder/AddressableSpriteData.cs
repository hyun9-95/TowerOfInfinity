using System.Collections.Generic;
using UnityEngine;

namespace TowerOfInfinity.CharacterBuilder
{
    [CreateAssetMenu(fileName = "AddressableSpriteData", menuName = "TowerOfInfinity/Addressable Sprite Data")]
    public class AddressableSpriteData : ScriptableObject
    {
        [System.Serializable]
        public class LayerData
        {
            public string LayerName;
            public List<string> Addresses; // Changed to a list of strings
        }

        public List<LayerData> Layers;
    }
}