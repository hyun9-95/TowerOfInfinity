using System.Collections.Generic;

public class UserCharacterPartsInfo
{
    public CharacterRace Race { get; private set; }
    public Dictionary<CharacterPartsType, DataCharacterParts> PartsInfoDic { get; private set; } = new();

    public void Initialize(UserSaveInfo userSaveInfo)
    {
        var partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();

        PartsInfoDic.Clear();

        // 종족별 고정파츠
        SetRaceParts(userSaveInfo.CharacterRace);

        // 헤어 파츠
        
        // 
    }

    private void SetRaceParts(CharacterRace race)
    {
        Race = race;
        
        string racePathName = GetRacePathName(race);
        
        if (string.IsNullOrEmpty(racePathName))
            return;

        var partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
        var racePartsTypes = new[] 
        { 
            CharacterPartsType.Head,
            CharacterPartsType.Ears,
            CharacterPartsType.Eyes,
            CharacterPartsType.Body,
            CharacterPartsType.Horns 
        };

        foreach (var partsType in racePartsTypes)
        {
            string expectedPath = $"CharacterBuilder/{partsType}/{racePathName}";
            
            var raceData = partsContainer.Find(data => 
                data.Category == CharacterPartsCategory.Race && 
                data.PartsType == partsType && 
                data.PartsPath == expectedPath);

            if (!raceData.IsNull)
            {
                PartsInfoDic[partsType] = raceData;
            }
        }
    }

    private string GetRacePathName(CharacterRace race)
    {
        return race switch
        {
            CharacterRace.Human => "Human",
            CharacterRace.Elf => "Elf",
            CharacterRace.DarkElf => "DarkElf",
            CharacterRace.Demon => "Demon",
            CharacterRace.LizardMan => "Lizard",
            CharacterRace.Merman => "Merman",
            CharacterRace.BeastMan => "Furry",
            CharacterRace.Orc => "Orc",
            CharacterRace.Goblin => "Goblin",
            CharacterRace.Vampire => "Vampire",
            CharacterRace.Undead => "Skeleton",
            CharacterRace.Monster => "ZombieA",
            _ => null
        };
    }
}
