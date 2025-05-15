using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CDebugger
{
    public static class CustomDebugger 
    {

        private const string LOGS_PATH = "Assets/Resources/CustomDebugger/CustomDebuggerSettings.asset";
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
            // Load settings using AssetDatabase in Editor
            _debuggerSettings = Resources.Load<DebuggerSettings>("CustomDebugger/CustomDebuggerSettings");


            if (_debuggerSettings == null)
            {
                Debug.LogWarning("LoggerSettings asset not found! Create one by opening settings window: Tools/CustomDebugger/Settings.");
            }
        }

        [HideInCallstack]
        private static void LogConsole(object type, object txt, LogType logType ,GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            EnsureInitialized();
            if (_debuggerSettings == null) return;

            var category = _debuggerSettings.GetCategory(type.ToString());
            if (category == null || !category.enabled) 
                return;
            
            var hexColor = ToHex(category.customColor);
            object header = $"<color={hexColor}> [<b>{type}</b>]:</color> ";
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
        public static void Log(object type,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(type, text, LogType.Log , sender);
#endif
        }
        [HideInCallstack] 
        public static void LogWarning(object type,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(type, text, LogType.Warning, sender);
#endif
        }
        [HideInCallstack] 
        public static void LogError(object type,object text, GameObject sender = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogConsole(type, text, LogType.Error, sender);
#endif
        }


        
        
    }
}
