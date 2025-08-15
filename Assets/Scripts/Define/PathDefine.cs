public class PathDefine
{
    #region const
    public const string Excel = "Assets/Data/Excels";
    public const string Json = "Data/Jsons";
    public const string DataClass = "Assets/Data/Scripts/Class";
    public const string Manager = "Assets/Scripts/Manager/";
    public const string DataContainerGenerator = "Assets/Data/Scripts/Container/DataContainerGenerator.cs";
    public const string DataContainer = "Assets/Data/Scripts/Container/";
    public const string VersionText = "Data/Jsons/Version.txt";
    public const string JsonListText = "Data/Jsons/JsonList.txt";
    public const string DefaultPrefabPath = "Prefab";
    public const string UIAddressableFullPath = "Assets/Addressable/UI";
    public const string ContentsScriptsFolderPath = "Assets/Scripts/Contents";
    public const string Addressable = "Assets/Addressable";
    public const string AddressablePathJson = "Assets/Addressable/BuildInfo/AddressablePath.json";
    public const string DefinePath = "Assets/Scripts/Public/Define";
    public const string EditorWindowPath = "Assets/Scripts/Editor/Window";
    public const string SkillPrefabPath = "Assets/Addressable/Skill/";
    public const string DataDefinePath = "Assets/Data/Scripts/Define";

    public const string PATH_USER_SAVE_INFO = "UserSaveInfo/{0}";
    
    public const string PATH_BATTLE_EVENT_BALANCE_FOLDER = "Assets/Addressable/AbilityCore/BattleEventBalance";
    public const string PATH_ABILITY_BALANCE_FOLDER = "Assets/Addressable/AbilityCore/AbilityBalance";
    public const string PATH_ADDRESSABLE_JSON_LIST = "Data/Jsons/JsonList";
    public const string PATH_ADDRESSABLE_JSON = "Data/Jsons/{0}";
    #endregion

    #region Resources
    public const string Resources_UI_View = "UI/{0}/{1}";
    public const string CHARACTER_INFO_MAIN = "CharacterCore/ScriptableCharacterInfo/MainCharacterInfo";
    public const string CHARACTER_INFO_SUB = "CharacterCore/ScriptableCharacterInfo/SubCharacterInfo";
    public const string CHARACTER_INFO_ENEMY = "CharacterCore/ScriptableCharacterInfo/EnemyCharacterInfo";
    public const string CHARACTER_INFO_ENEMY_BOSS = "CharacterCore/ScriptableCharacterInfo/EnemyBossCharacterInfo";
    public const string CHARACTER_INFO_NPC = "CharacterCore/ScriptableCharacterInfo/NpcCharacterInfo";
    public const string CHARACTER_EXP_GAINER = "BattleCore/BattleExpGainer";
    public const string BATTLE_EXP_GEM = "BattleCore/BattleExpGem";
    public const string DAMAGE_GROUP = "UI/Damages/DamageNumbersGroup";
    public const string NORMAL_DAMAGE = "UI/NormalDamage";
    public const string UI_VIEW_FORMAT = "UI/{0}/{0}";
    public const string CHARACTER_BUILDER_PARTS_FORMAT = "CharacterBuilder/{0}/{1}";
    public const string CHARACTER_MAIN_CHARACTER_PATH = "Character/Player/MainPlayerCharacter/MainPlayerCharacter";
    public const string CARD_TIER_FRAME_FORMAT = "UI/Battle/Card/BattleCardFrame{0}_{1}";
    #endregion

    #region property
    public static string AddressableBuildPathByPlatform
    {
        get
        {
#if UNITY_STANDALONE
            return "Addressable/StandaloneWindows64";
#elif UNITY_ANDROID
            return "Addressable/Android";
#else
            return "Addressable";
#endif
        }
    }

    public static string AddressableLoadPath
    {
        get
        {
            return $"{UnityEngine.Application.persistentDataPath}/{AddressableBuildPathByPlatform}";
        }
    }

    public static string AddressableBinPath
    {
        get
        {
            return $"Assets/AddressableAssetsData/{NameDefine.CurrentPlatformName}/addressables_content_state.bin";
        }
    }
#endregion
}

public class TemplatePathDefine
{
    public const string TemplatePath = "Assets/Templates/";

    public const string StartDataTemplate = "Assets/Templates/StartDataTemplate.txt";
    public const string EndDateTemplate = "Assets/Templates/EndDataTemplate.txt";
    public const string DataValueTemplate = "Assets/Templates/DataValueTemplate.txt";
    public const string StructValueTemplate = "Assets/Templates/StructValueTemplate.txt";
    public const string DataGeneratorTemplate = "Assets/Templates/DataGeneratorTemplate.txt";
    public const string AddContainerTypeTemplate = "Assets/Templates/AddContainerTypeTemplate.txt";
    public const string MVC_ControllerTemplate = "Assets/Templates/MVC_ControllerTemplate.txt";
    public const string MVC_ViewModelTemplate = "Assets/Templates/MVC_ViewModelTemplate.txt";
    public const string MVC_ViewTemplate = "Assets/Templates/MVC_ViewTemplate.txt";
    public const string UnitTemplate = "Assets/Templates/UnitTemplate.txt";
    public const string UnitModelTemplate = "Assets/Templates/UnitModelTemplate.txt";
    public const string ManagerTemplate = "Assets/Templates/ManagerTemplate.txt";
    public const string MonoManagerTemplate = "Assets/Templates/MonoManagerTemplate.txt";
    public const string UITypeTemplate = "Assets/Templates/UITypeTemplate.txt";
    public const string EditorWindowTemplate = "Assets/Templates/EditorWindowTemplate.txt";
    public const string DataDefine = "Assets/Templates/DataDefineTemplate.txt";
    
}

