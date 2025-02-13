using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomDebugger
{
    [Serializable]
    public class LogCategory
    {

        // public LogCategoryType type;
        public string name;
        public Color customColor;
        public bool enabled;
    }
}
