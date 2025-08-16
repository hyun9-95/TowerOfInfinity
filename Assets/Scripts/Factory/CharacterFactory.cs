using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : BaseManager<CharacterFactory>
{
    private Dictionary<CharacterType, ScriptableCharacterStateGroup> stateGroupByTypeDic = new();
    private Dictionary<string, ScriptableCharacterStateGroup> stateGroupByAddressDic = new();
    private Dictionary<CharacterType, ScriptableCharacterModuleGroup> moduleGroupByTypeDic = new();

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

        var model = CreateCharacterUnitModel(TeamTag.Ally, CharacterType.Sub, CharacterSetUpType.Battle, subCharacterInfo);
        model.SetCharacterDataId(subCharacterInfo.CharacterDataId);

        await SetCharacterScriptableInfo(subCharacter, CharacterType.Sub, subCharacterInfo.CharacterDataId);

        return subCharacter;
    }

    public async UniTask<CharacterUnit> SpawnEnemy(int characterDataId, Vector3 pos = default, Quaternion rot = default)
    {
        var data = DataManager.Instance.GetDataById<DataCharacter>(characterDataId);
        var enemy = await SpawnCharacter(data.PrefabName, null, pos, rot);

        if (enemy == null)
            return null;

        CharacterInfo enemyCharacterInfo = new SubCharacterInfo();
        enemyCharacterInfo.SetPrimaryWeaponAbility(data.PrimaryWeaponAbility);
        enemyCharacterInfo.SetActiveAbility(data.ActiveSkill);
        enemyCharacterInfo.SetPassiveAbility(data.PassiveSkill);

        var model = CreateCharacterUnitModel(TeamTag.Enemy, CharacterType.Enemy, CharacterSetUpType.Battle, enemyCharacterInfo);
        model.SetCharacterDataId(characterDataId);
        enemy.SetModel(model);

        await SetCharacterScriptableInfo(enemy, CharacterType.Enemy, characterDataId);

        return enemy;
    }

    public async UniTask<CharacterUnit> SpawnBoss(int characterDataId, Vector3 pos = default, Quaternion rot = default)
    {
        var data = DataManager.Instance.GetDataById<DataCharacter>(characterDataId);
        var enemy = await SpawnCharacter(data.PrefabName, null, pos, rot);

        if (enemy == null)
            return null;

        CharacterInfo enemyCharacterInfo = new SubCharacterInfo();
        enemyCharacterInfo.SetPrimaryWeaponAbility(data.PrimaryWeaponAbility);
        enemyCharacterInfo.SetActiveAbility(data.ActiveSkill);
        enemyCharacterInfo.SetPassiveAbility(data.PassiveSkill);

        var model = CreateCharacterUnitModel(TeamTag.Enemy, CharacterType.Boss, CharacterSetUpType.Battle, enemyCharacterInfo);
        model.SetCharacterDataId(characterDataId);
        model.SetIsEnablePhysics(false);
        enemy.SetModel(model);

        await SetCharacterScriptableInfo(enemy, CharacterType.Boss, characterDataId);

        return enemy;
    }

    public async UniTask<CharacterUnit> SpawnCharacter(string prefabPath, Transform transform = null, Vector3 pos = default, Quaternion rot = default)
    {
        var character = await InstantiateCharacter(prefabPath, transform, pos, rot);

        if (character == null)
            return null;

        return character;
    }

    private async UniTask<ScriptableCharacterStateGroup> GetScriptableStateGroupByType(CharacterType characterType)
    {
        if (!stateGroupByTypeDic.ContainsKey(characterType))
        {
            var scriptableCharacterInfo = await AddressableManager.Instance.
                LoadScriptableObject<ScriptableCharacterStateGroup>(GetCharacterStateGroupPath(characterType));
            stateGroupByTypeDic[characterType] = scriptableCharacterInfo;
        }

        return stateGroupByTypeDic[characterType];
    }

    private async UniTask<ScriptableCharacterStateGroup> GetScriptableStateGroup(string address)
    {
        if (!stateGroupByAddressDic.TryGetValue(address, out var stateGroup))
        {
            var newStateGroup = await AddressableManager.Instance.
                LoadScriptableObject<ScriptableCharacterStateGroup>(address);

            if (newStateGroup == null)
                return null;

            stateGroupByAddressDic[address] = newStateGroup;

            return newStateGroup;
        }

        return stateGroup;
    }

    private async UniTask<ScriptableCharacterModuleGroup> GetModuleGroupByType(CharacterType type)
    {
        if (!moduleGroupByTypeDic.TryGetValue(type, out var modueGroup))
        {
            var newModuleGroup = await AddressableManager.Instance.
                LoadScriptableObject<ScriptableCharacterModuleGroup>(GetCharacterModuleGroupPath(type));

            if (newModuleGroup == null)
                return null;

            moduleGroupByTypeDic[type] = newModuleGroup;
            return newModuleGroup;
        }

        return modueGroup;
    }

    private string GetCharacterStateGroupPath(CharacterType characterType)
    {
        return characterType switch
        {
            CharacterType.Main => PathDefine.STATE_GROUP_MAIN,
            CharacterType.Sub => PathDefine.STATE_GROUP_SUB,
            CharacterType.Enemy => PathDefine.STATE_GROUP_ENEMY,
            CharacterType.NPC => PathDefine.STATE_GROUP_NPC,
            CharacterType.Boss => PathDefine.STATE_GROUP_ENEMY_BOSS,
            _ => string.Empty,
        };
    }

    private string GetCharacterModuleGroupPath(CharacterType characterType)
    {
        return characterType switch
        {
            CharacterType.Main => PathDefine.MODULE_GROUP_PLAYER,
            CharacterType.Sub => PathDefine.MODULE_GROUP_PLAYER,
            CharacterType.Enemy => PathDefine.MODULE_GROUP_ENEMY,
            CharacterType.Boss => PathDefine.MODULE_GROUP_ENEMY,
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

        // 별도 스테이트 그룹이 없는 경우 캐릭터타입에 따른 기본 상태그룹을 넣어줌
        if (string.IsNullOrEmpty(character.CustomStateGroupAddress))
        {
            var defaultStateGroup = await GetScriptableStateGroupByType(characterType);
            character.SetStateGroup(defaultStateGroup);
        }
        else
        {
            // 캐릭터에서 정의하는 커스텀 상태 그룹
            var customStateGroup = await GetScriptableStateGroup(character.CustomStateGroupAddress);
            character.SetStateGroup(customStateGroup);

            if (customStateGroup is SoloScriptableStateGroup soloStateGroup)
                soloStateGroup.SetOwner(character);
        }

        var moduleGroup = await GetModuleGroupByType(characterType);
        character.SetModuleGroup(moduleGroup);
    }

    public void Clear()
    {
        // Release는 클리어 이후에 플로우 전환 시 이루어짐.
        stateGroupByTypeDic.Clear();
        stateGroupByAddressDic.Clear();
        moduleGroupByTypeDic.Clear();
    }
}