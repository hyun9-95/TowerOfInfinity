using UnityEngine;

public static class Logger
{
    private static readonly string separator = new('-', 50);
    public static void Log(string message, bool usePrefix = false)
    {
        DebugLog($"{(usePrefix ? "[Log] " : string.Empty)}{message}");
    }

    public static void BattleLog(string message)
    {
        DebugLog($"<color=#00BFFF>[Battle] {message}</color>");
    }

    public static void Success(string message)
    {
        DebugLog($"<color=cyan>[Success] {message}</color>");
    }

    public static void Warning(string message)
    {
        DebugLog($"<color=yellow>[Warning] {message}</color>");
    }

    public static void Exception(string message, System.Exception e)
    {
        Error(message);
        DebugLogError($"[Exception] {e}");
    }

    public static void Null(string nullObjectName)
    {
        DebugLogError($"[Null] {nullObjectName} is null");
    }

    public static void Error(string message)
    {
        DebugLogError($"[Error] {message}");
    }

    public static void Separator()
    {
        DebugLog(separator);
    }

    private static void DebugLog(string message)
    {
#if DEBUG_LOG || UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    private static void DebugLogError(string message)
    {
#if DEBUG_LOG || UNITY_EDITOR
        Debug.LogError(message);
#endif
    }
}