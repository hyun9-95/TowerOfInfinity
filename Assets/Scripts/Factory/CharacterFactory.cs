using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
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

        var subCharacter = await SpawnCharacter(prefabPath, transform, pos, rot);

        if (subCharacter == null)
            return null;

        await SetCharacterScriptableInfo(subCharacter, CharacterType.Sub, subCharacterInfo.CharacterDataId);

        var model = CreateCharacterUnitModel(TeamTag.Ally, CharacterType.Sub, CharacterSetUpType.Battle, subCharacterInfo);
        model.SetCharacterDataId(subCharacterInfo.CharacterDataId);

        return subCharacter;
    }

    public async UniTask<CharacterUnit> SpawnEnemy(int characterDataId, Vector3 pos = default, Quaternion rot = default)
    {
        var data = DataManager.Instance.GetDataById<DataCharacter>(characterDataId);
        var enemy = await SpawnCharacter(data.PrefabName, null, pos, rot);

        if (enemy == null)
            return null;

        await SetCharacterScriptableInfo(enemy, CharacterType.Enemy, characterDataId);

        CharacterInfo enemyCharacterInfo = new SubCharacterInfo();
        enemyCharacterInfo.SetPrimaryWeaponAbility(data.PrimaryWeaponAbility);
        enemyCharacterInfo.SetActiveAbility(data.ActiveSkill);
        enemyCharacterInfo.SetPassiveAbility(data.PassiveSkill);

        var model = CreateCharacterUnitModel(TeamTag.Enemy, CharacterType.Enemy, CharacterSetUpType.Battle, enemyCharacterInfo);
        model.SetCharacterDataId(characterDataId);
        enemy.SetModel(model);

        return enemy;
    }

    public async UniTask<CharacterUnit> SpawnCharacter(string prefabPath, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        var character = await InstantiateCharacter(prefabPath, transform, pos, rot);

        if (character == null)
            return null;

        return character;
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
            CharacterType.NPC => PathDefine.CHARACTER_INFO_NPC,
            _ => string.Empty,
        };
    }

    public CharacterUnitModel CreateCharacterUnitModel(TeamTag teamTag, CharacterType characterType, CharacterSetUpType setUpType = CharacterSetUpType.Town, CharacterInfo characterInfo = null)
    {
        CharacterSetUpType characterSetUpType = FlowManager.Instance.CurrentFlowType == FlowType.BattleFlow ?
            CharacterSetUpType.Battle : CharacterSetUpType.Town;

        CharacterUnitModel characterModel = new CharacterUnitModel();
        SetCharacterUnitModel(characterModel, teamTag, characterType, setUpType, characterInfo);

        return characterModel;
    }

    public void SetCharacterUnitModel(CharacterUnitModel characterModel,TeamTag teamTag, CharacterType characterType, CharacterSetUpType setUpType = CharacterSetUpType.Town, CharacterInfo characterInfo = null)
    {
        CharacterSetUpType characterSetUpType = FlowManager.Instance.CurrentFlowType == FlowType.BattleFlow ?
            CharacterSetUpType.Battle : CharacterSetUpType.Town;

        characterModel.SetTeamTag(teamTag);
        characterModel.SetCharacterType(characterType);
        characterModel.SetCharacterSetUpType(setUpType);

        // CharacterInfo 설정
        if (characterInfo != null)
            characterModel.SetCharacterInfo(characterInfo);
    }

    public async UniTask SetCharacterScriptableInfo(CharacterUnit character, CharacterType characterType, int dataId = 0)
    {
        if (character == null)
            return;

        ScriptableCharacterInfo scriptableCharacterInfo = null;

        if (dataId != 0)
        {
            var data = DataManager.Instance.GetDataById<DataCharacter>(dataId);

            if (!data.IsNull && !string.IsNullOrEmpty(data.CustomCharacterInfo))
            {
                var path = data.CustomCharacterInfo;
                var customCharacterInfo = await AddressableManager.Instance.
                    LoadAssetAsyncWithTracker<ScriptableCharacterInfo>(path, character.gameObject);

                if (customCharacterInfo != null)
                    scriptableCharacterInfo = customCharacterInfo;
            }
        }

        if (scriptableCharacterInfo == null)
            scriptableCharacterInfo = await GetScriptableCharacterInfo(characterType);

        character.SetStateGroup(scriptableCharacterInfo.StateGroup);
        character.SetModuleGroup(scriptableCharacterInfo.ModuleGroup);
    }

    public void Clear()
    {
        scriptableCharacterInfoDic.Clear();
    }
}