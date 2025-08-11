using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class User
{
    #region Property
    public int ID { get; set; }
    public UserCharacterInfo UserCharacterInfo { get; private set; }
    public UserEquipmentInfo UserEquipmentInfo { get; private set; }
    public SceneDefine CurrentTown { get; private set; }
    
    // 기본 정보
    public string UserId { get; private set; }
    public bool IsCompletePrologue { get; set; }
    #endregion

    #region Value

    #endregion

    #region Function
    public void CreateUserByUserSaveInfo(UserSaveInfo userSaveInfo)
    {
        UserId = userSaveInfo.Id;
        IsCompletePrologue = userSaveInfo.IsCompletePrologue;
        CurrentTown = userSaveInfo.CurrentTown;

#if CHEAT
        CurrentTown = CheatManager.CheatConfig.SceneDefine;
#endif

        UserCharacterInfo = new UserCharacterInfo();

        CreateEquipmentInfo(userSaveInfo);
        CreateMainCharacterInfo(userSaveInfo);
        CreateSubCharacters(userSaveInfo);
    }

    private void CreateSubCharacters(UserSaveInfo userSaveInfo)
    {
        var subCharacterInfos = new SubCharacterInfo[userSaveInfo.SubCharacterDataIds.Count];

        userSaveInfo.SubCharacterDataIds.Sort();

        for (int i = 0; i < userSaveInfo.SubCharacterDataIds.Count; i++)
        {
            int dataCharacterId = userSaveInfo.SubCharacterDataIds[i];

            int slotIndex = userSaveInfo.SubCharacterSlotIndexDic.ContainsKey(dataCharacterId) ?
                userSaveInfo.SubCharacterSlotIndexDic[dataCharacterId] : -1;

            var subCharacterData = DataManager.Instance.GetDataContainer<DataCharacter>().GetById(dataCharacterId);

            SubCharacterInfo userCharacter = new SubCharacterInfo();
            userCharacter.SetCharacterDataId(dataCharacterId);
            userCharacter.SetPrimaryWeaponAbility(subCharacterData.PrimaryWeaponAbility);
            userCharacter.SetActiveAbility(subCharacterData.ActiveSkill);
            userCharacter.SetPassiveAbility(subCharacterData.PassiveSkill);
            userCharacter.SetSlotIndex(slotIndex);

            subCharacterInfos[i] = userCharacter;
        }

        UserCharacterInfo.SetSubCharacterInfos(subCharacterInfos);
        UserCharacterInfo.UpdateCurrentDeck();
    }

    private void CreateMainCharacterInfo(UserSaveInfo userSaveInfo)
    {
        var mainCharacterInfo = new MainCharacterInfo();
        mainCharacterInfo.SetCharacterRace(userSaveInfo.CharacterRace);
        mainCharacterInfo.SetHairPartsId(userSaveInfo.HairPartsId);

        var mainCharacterPartsInfo = new MainCharacterPartsInfo();
        mainCharacterPartsInfo.SetRaceParts(mainCharacterInfo.CharacterRace);
        mainCharacterPartsInfo.SetHairParts(mainCharacterInfo.HairPartsId);
        var equippedEquipments = UserEquipmentInfo.CreateEquippedMainCharacterEquipments(userSaveInfo);
        mainCharacterInfo.SetEquippedEquipments(equippedEquipments);
        
        mainCharacterPartsInfo.SetEquipmentParts(mainCharacterInfo.GetEquipmentDefines());

        mainCharacterInfo.SetPartsInfo(mainCharacterPartsInfo);

        var equippedWeapon = mainCharacterInfo.GetEquippedEquipment(EquipmentType.Weapon);

        if (equippedWeapon != null)
            mainCharacterInfo.SetPrimaryWeaponAbility(equippedWeapon.Ability);

        UserCharacterInfo.SetMainCharacterInfo(mainCharacterInfo);
    }

    private void CreateEquipmentInfo(UserSaveInfo userSaveInfo)
    {
        UserEquipmentInfo = new UserEquipmentInfo();
        UserEquipmentInfo.CreateFromUserSaveInfo(userSaveInfo);
    }

    public void SetCompletePrologue(bool value)
    {
        IsCompletePrologue = value;
    }

    public void SetCurrentTown(SceneDefine town)
    {
        // 추후 마을은 데이터로 분리하자.
        CurrentTown = town;
    }

    public void Save()
    {
        var userSaveInfo = CreateUserSaveInfo();

        string userSaveInfoPath = Path.Combine(Application.persistentDataPath,
                                        string.Format(PathDefine.PATH_USER_SAVE_INFO, NameDefine.UserSaveInfo));

        string directory = Path.GetDirectoryName(userSaveInfoPath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var saveInfoJson = JsonConvert.SerializeObject(userSaveInfo);
        File.WriteAllText(userSaveInfoPath, saveInfoJson);
    }

    private UserSaveInfo CreateUserSaveInfo()
    {
        var userSaveInfo = new UserSaveInfo();

        // 기본 정보
        userSaveInfo.SetId(UserId);
        userSaveInfo.SetIsCompletePrologue(IsCompletePrologue);

        // 서브 캐릭터
        var subCharacterDataIds = new List<int>();
        var subCharacterSlotIndexDic = new Dictionary<int, int>();

        if (UserCharacterInfo.SubCharacterInfos != null)
        {
            foreach (var subCharacter in UserCharacterInfo.SubCharacterInfos)
            {
                if (subCharacter == null)
                    continue;

                subCharacterDataIds.Add(subCharacter.CharacterDataId);

                if (subCharacter.SlotIndex >= 0)
                    subCharacterSlotIndexDic[subCharacter.CharacterDataId] = subCharacter.SlotIndex;
            }
        }
        
        userSaveInfo.SetSubCharacterDataIds(subCharacterDataIds);
        userSaveInfo.SetSubCharacterSlotIndexDic(subCharacterSlotIndexDic);

        // 메인 캐릭터
        var characterRace = UserCharacterInfo.MainCharacterInfo.CharacterRace;
        var hairPartsId = UserCharacterInfo.MainCharacterInfo.HairPartsId;
        
        userSaveInfo.SetCharacterRace(characterRace);
        userSaveInfo.SetHairPartsId(hairPartsId);

        // 장비
        var ownedEquipmentIds = new List<int>();
        var equipmentLevels = new Dictionary<int, int>();

        foreach (var equipment in UserEquipmentInfo.UserEquipments.Values)
        {
            ownedEquipmentIds.Add(equipment.DataId);
            equipmentLevels[equipment.DataId] = equipment.Level;
        }

        userSaveInfo.SetOwnedEquipmentIds(ownedEquipmentIds);
        userSaveInfo.SetEquipmentLevels(equipmentLevels);
        
        var equippedMainCharacterEquipmentIds = new Dictionary<EquipmentType, int>();

        foreach (var kvp in UserCharacterInfo.MainCharacterInfo.EquippedEquipments)
            equippedMainCharacterEquipmentIds[kvp.Key] = kvp.Value.DataId;

        userSaveInfo.SetEquippedMainCharacterEquipmentIds(equippedMainCharacterEquipmentIds);

        userSaveInfo.SetCurrentTown(CurrentTown);

        userSaveInfo.CheckDefaultValue();
        
        return userSaveInfo;
    }
    #endregion
}
