using System.Collections.Generic;
using UnityEngine;

namespace TowerOfInfinity.CharacterBuilder
{
    [CreateAssetMenu(fileName = "AddressableSpriteData", menuName = "TowerOfInfinity/Addressable Sprite Data")]
    public class AddressableSpriteData : ScriptableObject
    {
        [System.Serializable]
        public class PartData
        {
            public string PartName;
            public string Address;
        }

        [System.Serializable]
        public class LayerEntry // LayerData 대신 LayerEntry로 이름 변경 (혼동 방지)
        {
            public string LayerName;
            public List<PartData> Parts; // PartsData 대신 Parts로 이름 변경 (간결성)
        }

        // 이 리스트가 인스펙터에 표시됩니다。
        public List<LayerEntry> LayerEntries = new List<LayerEntry>();
    }
}