public class NameDefine 
{
    public const string BucketDefaultName = "towerofinfinity-68c7d";
    public const string JsonListTxtName = "JsonList.txt";
    public const string JsonVersionTxtName = "JsonVersion.txt";
    public const string AddressableBuildInfoName = "AddressableBuildInfo.json";
    public const string AddressableVersionTxtName = "AddressableVersion.txt";
    public const string AddressablePathName = "AddressablePath.json";
    public const string UITypeDefineScriptName = "UITypeDefine.cs";
    public const string SimpleTextUnitName = "SimpleTextUnit";
    public const string AddressableDefaultGroupName = "Default Local Group";
    public const string UploadHistory = "UploadHistory.txt";
    public const string AddressableDefaultGroupName_Newer = "Default";
    public const string UserSaveInfo = "userSaveInfo.json";

    public static string CurrentPlatformName
    {
        get 
        {
            string platform;

#if UNITY_STANDALONE
            platform = "Windows";
#elif UNITY_ANDROID
            platform = "Android";
#endif
            return platform;
        } 
    }
}
