using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : BaseManager<CharacterFactory>
{
    private Dictionary<CharacterType, ScriptableCharacterInfo> scriptableCharacterInfoDic = new Dictionary<CharacterType, ScriptableCharacterInfo>();
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

    public async UniTask AddExpGainer(CharacterUnit playerCharacter)
    {
        if (!playerCharacter.gameObject.TryGetComponent<BattleExpGainer>(out var expGainer))
            expGainer = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BattleExpGainer>(PathDefine.CHARACTER_EXP_GAINER, playerCharacter.transform);
        
    }

    public async UniTask<CharacterUnit> SpawnSubCharacter(SubCharacterInfo subCharacterInfo, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        DataCharacter dataCharacter = DataManager.Instance.GetDataById<DataCharacter>(subCharacterInfo.CharacterDataId);

        var prefabPath = dataCharacter.PrefabName;

        int weaponDataId = subCharacterInfo.PrimaryWeaponDataId;
        int activeSkillDataId = subCharacterInfo.ActiveSkillDataId;
        int passiveSkillDataId = subCharacterInfo.PassiveSkillDataId;

        var subCharacter = await SpawnCharacter
            (TeamTag.Ally, CharacterType.Sub, prefabPath, transform, pos, rot);

        if (subCharacter != null)
            subCharacter.Model.SetCharacterDataId(subCharacterInfo.CharacterDataId);

        return subCharacter;
    }

    public async UniTask<CharacterUnit> SpawnEnemy(int characterDataId, Vector3 pos = default, Quaternion rot = default)
    {
        var data = DataManager.Instance.GetDataById<DataCharacter>(characterDataId);
        var enemy = await SpawnCharacter(TeamTag.Enemy, CharacterType.Enemy, data.PrefabName, null, pos, rot);

        if (enemy != null)
            enemy.Model.SetCharacterDataId(characterDataId);

        return enemy;
    }

    public async UniTask<CharacterUnit> SpawnCharacter(TeamTag teamTag, CharacterType characterType, string prefabPath, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        var character = await InstantiateCharacter(prefabPath, transform, pos, rot);

        if (character == null)
            return null;

        bool result = await SetCharacterUnitModel(character, teamTag, characterType);

        return result ? character : null;
    }

    private async UniTask<ScriptableCharacterInfo> GetScriptableCharacterInfo(CharacterType characterType)
    {
        if (!scriptableCharacterInfoDic.ContainsKey(characterType))
        {
            var scriptableCharacterInfo = await AddressableManager.Instance.LoadScriptableObject<ScriptableCharacterInfo>(GetCharacterInfoPath(characterType));
            scriptableCharacterInfoDic[characterType] = scriptableCharacterInfo;
        }

        return scriptableCharacterInfoDic[characterType];
    }

    private string GetCharacterInfoPath(CharacterType characterType)
    {
        return characterType switch
        {
            CharacterType.Main => PathDefine.CHARACTER_INFO_MAIN,
            CharacterType.Sub => PathDefine.CHARACTER_INFO_SUB,
            CharacterType.Enemy => PathDefine.CHARACTER_INFO_ENEMY,
            CharacterType.EnemyBoss => PathDefine.CHARACTER_INFO_ENEMY_BOSS,
            CharacterType.NPC => PathDefine.CHARACTER_INFO_NPC,
            _ => string.Empty,
        };
    }

    public async UniTask<bool> SetCharacterUnitModel(CharacterUnit character, TeamTag teamTag, CharacterType characterType)
    {
        #region Model Set
        CharacterSetUpType characterSetUpType = FlowManager.Instance.CurrentFlowType == FlowType.BattleFlow ?
            CharacterSetUpType.Battle : CharacterSetUpType.Town;

        CharacterUnitModel characterModel = new CharacterUnitModel();
        characterModel.SetTeamTag(teamTag);
        characterModel.SetCharacterType(characterType);
        characterModel.SetCharacterSetUpType(characterSetUpType);  

        #endregion

        if (character != null)
        {
            character.SetModel(characterModel);

            var characterInfo = await GetScriptableCharacterInfo(characterType);
            character.SetStateGroup(characterInfo.StateGroup);
            character.SetModuleGroup(characterInfo.ModuleGroup);
        }

        return true;
    }

    public void Clear()
    {
        scriptableCharacterInfoDic.Clear();
    }
}