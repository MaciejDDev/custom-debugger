using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomDebugger
{
    [CreateAssetMenu(fileName = "DebuggerSettings", menuName = "CustomDebuggerSettings")]
    public class DebuggerSettings : ScriptableObject
    {
        public List<LogCategory> categories;

        // public LogCategory GetLogData(LogCategory feature)
        // {
        //     if (_features == null)
        //     {
        //         return null;
        //     }
        //
        //     var featureData = _features.Find((element) => element.feature == feature);
        //
        //     return featureData;
        // }
        public LogCategory GetCategory(LogCategoryType categoryName)
        {
            return categories.Find(cat => cat.name == categoryName.ToString());
        }

        public void SetCategoryEnabled(LogCategoryType type, bool enabled)
        {
            LogCategory category = GetCategory(type);
            if (category != null)
                category.enabled = enabled;
        }

        public void SetAllLogging(bool enabled)
        {
            foreach (var category in categories)
            category.enabled = enabled;
        }
    }
}