using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CDebugger
{
    public static class CustomDebugger 
    {

        private const string LOGS_PATH = "Packages/com.maciejddev.custom-debugger/Runtime/Assets/CustomDebuggerSettings.asset";
        private static DebuggerSettings _debuggerSettings;
        private static bool _isInitialized = false;


        enum LogType
        {
            Log,
            Warning,
            Error
        }
        private static void LoadSettings()
        {
#if UNITY_EDITOR
            // Load settings using AssetDatabase in Editor
            _debuggerSettings = AssetDatabase.LoadAssetAtPath<DebuggerSettings>(LOGS_PATH);
#else
        // Load settings using Resources in Development Builds
        _logSettings = Resources.Load<LogSettings>("CustomDebuggerSettings.asset");
#endif

            if (_debuggerSettings == null)
            {
                Debug.LogWarning("LoggerSettings asset not found! Create one in Tools/CustomDebugger/Settings.");
            }
        }

        [HideInCallstack]
        private static void LogConsole(LogCategoryType type, object txt, LogType logType ,GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            EnsureInitialized();
            if (_debuggerSettings == null) return;

            var category = _debuggerSettings.GetCategory(type);
            if (category == null || !category.enabled) 
                return;
            
            var hexColor = ToHex(category.customColor);
            object header = $"[<color={hexColor}> <b>{type}</b>]:</color> ";
            switch (logType)
            {
                case LogType.Warning:
                    Debug.LogWarning($"{header} {txt}", sender);
                    break;
                case LogType.Error:
                    Debug.LogError($"{header} {txt}", sender);
                    break;
                case LogType.Log:
                default:
                Debug.Log($"{header} {txt}", sender);
                    break;
                
            }
            //Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null,"{0} {1}\n{2}", header, txt, formattedStackTrace);
#endif

        }

        private static void EnsureInitialized()
        {
            if (_isInitialized) return;
            LoadSettings();
            _isInitialized = true;
        }

        private static string ToHex(Color color)
        {
            var col = (Color32)color;
            return $"#{col.r.ToString("x2")}{col.g.ToString("x2")}{col.b.ToString("x2")}{col.a.ToString("x2")}";
        }

        // public static void EnableLogging(bool enable)
        // {
        //     if (_debuggerSettings == null) return;
        //     _debuggerSettings.SetAllLogging(enable);
        // }

        // public static void ToggleCategory(LogCategory category, bool enable)
        // {
        //     if (_debuggerSettings == null) return;
        //     _debuggerSettings.SetCategoryEnabled(category., enable);
        // }

        [HideInCallstack] 
        public static void Log(LogCategoryType feature,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(feature, text, LogType.Log , sender);
#endif
        }
        [HideInCallstack] 
        public static void LogWarning(LogCategoryType feature,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(feature,text, LogType.Warning, sender);
#endif
        }
        [HideInCallstack] 
        public static void LogError(LogCategoryType feature,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(feature,text, LogType.Error, sender);
#endif
        }


        
        
    }
}
