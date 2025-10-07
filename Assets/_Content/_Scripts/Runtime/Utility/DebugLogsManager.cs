using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class DebugLogsManager
{
    [Conditional("DEBUG_LOGS")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Conditional("DEBUG_LOGS")]
    public static void Log(string message, UnityEngine.Object context)
    {
        Debug.Log(message, context);
    }

    [Conditional("DEBUG_LOGS")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("DEBUG_LOGS")]
    public static void LogWarning(string message, UnityEngine.Object context)
    {
        Debug.LogWarning(message, context);
    }

    [Conditional("DEBUG_LOGS")]
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    [Conditional("DEBUG_LOGS")]
    public static void LogError(string message, UnityEngine.Object context)
    {
        Debug.LogError(message, context);
    }
}
