using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CDebugger
{
    public class CustomDebuggerEditor : EditorWindow
    {
        private DebuggerSettings _debuggerSettings;
        private string newCategoryName = "NewCategory";
        private Color newCategoryColor = Color.white;
        

        private bool _needsEnumUpdate = false;
        
        [MenuItem("Tools/CustomDebugger/Settings")]
        public static void ShowWindow()
        {
            GetWindow<CustomDebuggerEditor>("Custom Debugger Settings");
        }
        private void OnEnable() => LoadSettings();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Custom Debugger Settings.", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _debuggerSettings.categories ??= new();
            UpdateDebuggerSettings();
            if (_needsEnumUpdate)
            {
                UpdateLogCategoryEnum();
                _needsEnumUpdate = false;
            }

            if (GUILayout.Button("Enable All Categories"))
                _debuggerSettings.SetAllLogging(true);
            if (GUILayout.Button("Disable All Categories"))
                _debuggerSettings.SetAllLogging(false);
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("Log Categories", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            var categoriesCopy = new List<LogCategory>(_debuggerSettings.categories);
            foreach (var category in categoriesCopy)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(10);
                category.enabled = EditorGUILayout.Toggle(category.enabled, GUILayout.Width(20));
                EditorGUILayout.LabelField(category.name,EditorStyles.boldLabel);
                category.customColor = EditorGUILayout.ColorField(category.customColor);
                string enabledLabel =  category.enabled ? "Enabled" : "Disabled";
                // Display "Enabled" (Green) or "Disabled" (Red)
                // Create a GUIStyle to change the text color
                GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
                statusStyle.normal.textColor = category.enabled ? Color.green : Color.red;
                EditorGUILayout.LabelField(enabledLabel, statusStyle, GUILayout.Width(60));
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    ShowDeleteWarning(category);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(10);

            GUILayout.Label("Add New Category", EditorStyles.boldLabel);

            newCategoryName = EditorGUILayout.TextField("Name", newCategoryName);
            newCategoryColor = EditorGUILayout.ColorField("Color", newCategoryColor);
            if (GUILayout.Button("Create"))
            {
                if (!_debuggerSettings.categories.Any(c => c.name == newCategoryName) && !string.IsNullOrWhiteSpace(newCategoryName))
                {
                    _debuggerSettings.categories.Add(new LogCategory() { name = newCategoryName, customColor = newCategoryColor, enabled = true });
                    UpdateLogCategoryEnum();
                    newCategoryName = "NewCategory"; // Reset input field
                    newCategoryColor = Color.white;
                }
                else
                {
                    Debug.LogWarning("Category already exists!");
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_debuggerSettings);
                AssetDatabase.SaveAssets();
            }
        }

        private void ShowDeleteWarning(LogCategory category)
        {
            // string category = debugger.logCategories[index];

            bool confirmDelete = EditorUtility.DisplayDialog(
                "Warning: Deleting Log Category",
                $"If any script references '{category.name}', your project will fail to compile until you update or remove those references.\n\nAre you sure you want to delete this category?",
                "Delete",
                "Cancel"
            );

            if (confirmDelete)
            {
                _debuggerSettings.categories.Remove(category);
                EditorUtility.SetDirty(_debuggerSettings);
                Debug.Log("Remove category: " + category.name);
                UpdateLogCategoryEnum();
            }
        }
        
        private void UpdateDebuggerSettings()
        {
            var enumValues = Enum.GetValues(typeof(LogCategoryType));
            //Debug.Log($"Updating debugger settings. debugger settings categories count: {_debuggerSettings.categories.Count} and enum length: {enumValues.Length} ");
            if (_debuggerSettings.categories.Count != enumValues.Length)
            {
                bool needsUpdate = false;
                foreach (var category in _debuggerSettings.categories)
                {
                    if (!Enum.IsDefined(typeof(LogCategoryType), category.name))
                    {
                        needsUpdate = true;
                        break;
                    }
                }

                if (needsUpdate)
                {
                    _needsEnumUpdate = true;  // Defer the update to be done once
                }
            }
        }

        void UpdateLogCategoryEnum()
        {
            string filePath = "Packages/com.MaciejDDev.CustomDebugger/Runtime/LogCategoryType.cs";
    
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Create the file if it does not exist
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Create the file content as it's being created
                Debug.Log("Creating LogCategoryType.cs because it doesn't exist.");
            }

            // Start building the enum content
            StringBuilder enumContent = new StringBuilder();
            enumContent.AppendLine("namespace Tools.CDebugger");
            enumContent.AppendLine("{");

            enumContent.AppendLine("    public enum LogCategoryType");
            enumContent.AppendLine("    {");

            // Add categories to the enum
            foreach (var category in _debuggerSettings.categories)
            {
                enumContent.AppendLine($"       {category.name.Replace(" ", "_")},");
            }

            enumContent.AppendLine("    }");
            enumContent.AppendLine("}");

            // Write the content to the file (this will overwrite existing file)
            File.WriteAllText(filePath, enumContent.ToString());
            //Debug.Log("Updated LogCategoryType.cs");

            // Refresh the AssetDatabase to reflect changes in the editor
            AssetDatabase.Refresh();
        }


        private void LoadSettings()
        {
            _debuggerSettings = AssetDatabase.LoadAssetAtPath<DebuggerSettings>(
                "Packages/com.maciejddev.custom-debugger/Runtime/Assets/CustomDebuggerSettings.asset");
            if (_debuggerSettings == null)
            {
                Debug.LogWarning("Custom Debugger settings not found! Creating new one...");
                CreateSettingsAsset();
            }
        }

        private void CreateSettingsAsset()
        {
            _debuggerSettings = CreateInstance<DebuggerSettings>();
            string packagePath = "Packages/com.maciejddev.custom-debugger/Runtime/Assets/CustomDebuggerSettings.asset";

            // Crear el asset dentro del paquete
            AssetDatabase.CreateAsset(_debuggerSettings, packagePath);
            UpdateDebuggerSettings();
            AssetDatabase.SaveAssets();
            
            // AssetDatabase.CreateAsset(_debuggerSettings, "Assets/Resources/CustomDebugger/CustomDebuggerSettings.asset");
            // UpdateDebuggerSettings();
            // AssetDatabase.SaveAssets();
        }
        
        
        
    }
}