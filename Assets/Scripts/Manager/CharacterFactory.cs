using Cysharp.Threading.Tasks;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : BaseManager<CharacterFactory>
{
    private Dictionary<CharacterType, ScriptableCharacterInfo> characterInfoDic = new Dictionary<CharacterType, ScriptableCharacterInfo>();
    
    /// <summary>
    /// tr 없으면 pooling
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="tr"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    private async UniTask<CharacterUnit> InstantiateCharacter(string prefabName, Transform tr = null, Vector3 pos = default, Quaternion rot = default)
    {
        CharacterUnit character = null;

        if (tr == null)
        {
            character = await ObjectPoolManager.Instance.SpawnPoolableMono<CharacterUnit>(prefabName, pos, rot);
        }
        else
        {
            character = await AddressableManager.Instance.InstantiateAddressableMonoAsync<CharacterUnit>(prefabName, tr);
        }
        
        if (character == null)
        {
            Logger.Null($"{prefabName}");
            return null;
        }

        character.transform.SetPositionAndRotation(pos, rot);

        return character;
    }

    public async UniTask<CharacterUnit> SpawnLeaderPlayerCharacter(int characterDataId, int weaponDataId = 0, int activeId = 0, int passiveId = 0, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        var playerCharacter = await SpawnCharacter(TeamTag.Ally, CharacterType.Leader, characterDataId, weaponDataId, activeId, passiveId, transform, pos, rot);

        if (playerCharacter != null)
        {
            var input = playerCharacter.gameObject.AddComponent<PlayerCharacterInput>();
            input.Initialize(playerCharacter.Model);

            if (FlowManager.Instance.CurrentFlowType == FlowType.BattleFlow)
            {
                var expGainer = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BattleExpGainer>(PathDefine.CHARACTER_EXP_GAINER, playerCharacter.transform);
                var expGainerModel = new BattleExpGainerModel();
                expGainer.SetModel(expGainerModel);
                expGainer.Activate(false);
            }
        }

        return playerCharacter;
    }

    public async UniTask<CharacterUnit> SpawnServent(int characterDataId, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        var servent = await SpawnCharacter(TeamTag.Ally, CharacterType.Servent, characterDataId, 0, 0, 0, transform, pos, rot);

        return servent;
    }

    public async UniTask<CharacterUnit> SpawnEnemy(int characterDataId, Vector3 pos = default, Quaternion rot = default)
    {
        var enemy = await SpawnCharacter(TeamTag.Enemy, CharacterType.Enemy, characterDataId, 0, 0, 0, null, pos, rot);

        return enemy;
    }

    public async UniTask<CharacterUnit> SpawnCharacter(TeamTag teamTag, CharacterType characterType, int characterDataId, int defaultWeaponId = 0, int activeId = 0, int passiveId = 0, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        DataCharacter dataCharacter = DataManager.Instance.GetDataById<DataCharacter>(characterDataId);

        var prefabName = dataCharacter.PrefabName;
        var character = await InstantiateCharacter(prefabName, transform, pos, rot);

        if (character == null)
            return null;

        #region Model Set
        CharacterUnitModel characterModel = new CharacterUnitModel();
        characterModel.SetCharacterDataId(characterDataId);
        characterModel.SetTeamTag(teamTag);

        if (defaultWeaponId == 0)
            defaultWeaponId = (int)dataCharacter.Default_Weapon;

        if (activeId == 0)
            activeId = (int)dataCharacter.ActiveSkill;

        if (passiveId == 0)
            passiveId = (int)dataCharacter.PassiveSkill;

        if (FlowManager.Instance.CurrentFlowType == FlowType.BattleFlow)
        {
            // 캐릭터가 가지고 있는 기본 무기, 기본 스킬들 설정
            Weapon weapon = AbilityFactory.Create<Weapon>(defaultWeaponId, characterModel);

            if (weapon != null)
                characterModel.SetDefaultWeapon(weapon);

            ActiveSkill activeSkill = AbilityFactory.Create<ActiveSkill>(activeId, characterModel);

            if (activeSkill != null)
                characterModel.SetActiveSkill(activeSkill);

            PassiveSkill passiveSkill = AbilityFactory.Create<PassiveSkill>(passiveId, characterModel);

            if (passiveSkill != null)
                characterModel.SetPassiveSkill(passiveSkill);
        }
        #endregion

        if (character != null)
        {
            character.SetModel(characterModel);

            var characterInfo = await GetCharacterInfo(characterType);
            character.SetStateGroup(characterInfo.StateGroup);
            character.SetModuleGroup(characterInfo.ModuleGroup);
        }

        return character;
    }

    private async UniTask<ScriptableCharacterInfo> GetCharacterInfo(CharacterType characterType)
    {
        if (!characterInfoDic.ContainsKey(characterType))
            characterInfoDic[characterType] = await AddressableManager.Instance.LoadScriptableObject<ScriptableCharacterInfo>(GetCharacterInfoPath(characterType));

        return characterInfoDic[characterType];
    }

    private string GetCharacterInfoPath(CharacterType characterType)
    {
        return characterType switch
        {
            CharacterType.Leader => PathDefine.CHARACTER_INFO_LEADER,
            CharacterType.Servent => PathDefine.CHARACTER_INFO_SERVENT,
            CharacterType.Enemy => PathDefine.CHARACTER_INFO_ENEMY,
            CharacterType.EnemyBoss => PathDefine.CHARACTER_INFO_ENEMY_BOSS,
            CharacterType.NPC => PathDefine.CHARACTER_INFO_NPC,
            _ => string.Empty,
        };
    }
}