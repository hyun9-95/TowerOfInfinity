public class PathDefine
{
    #region const
    public const string Excel = "Assets/Data/Excels";
    public const string Json = "Assets/Data/Jsons";
    public const string DataStruct = "Assets/Data/Scripts/Structs";
    public const string Manager = "Assets/Scripts/Manager/";
    public const string DataContainerGenerator = "Assets/Data/Scripts/Container/DataContainerGenerator.cs";
    public const string DataContainer = "Assets/Data/Scripts/Container/";
    public const string VersionText = "Assets/Data/Jsons/Version.txt";
    public const string JsonListText = "Assets/Data/Jsons/JsonList.txt";
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
    #endregion

    #region Resources
    public const string Resources_UI_View = "UI/{0}/{1}";
    public const string CHARACTER_INFO_LEADER = "LeaderCharacterInfo";
    public const string CHARACTER_INFO_SERVENT = "ServentCharacterInfo";
    public const string CHARACTER_INFO_ENEMY = "EnemyCharacterInfo";
    public const string CHARACTER_INFO_ENEMY_BOSS = "EnemyBossCharacteInfo";
    public const string CHARACTER_INFO_NPC = "NpcCharacterInfo";
    public const string CHARACTER_EXP_GAINER = "BattleExpGainer";
    public const string BATTLE_EXP_GEM = "BattleExpGem";
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

