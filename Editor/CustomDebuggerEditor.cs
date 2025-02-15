using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CDebugger
{
    public class CustomDebuggerEditor : EditorWindow
    {
        private DebuggerSettings _debuggerSettings;
        private string newCategoryName = "NewCategory";
        private Color newCategoryColor = Color.white;
        

        // private bool _needsEnumUpdate = false;
        
        

        
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

            if (_debuggerSettings == null)
                LoadSettings();
            
            
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
                    UpdateLogCategoriesClass();
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
                UpdateLogCategoriesClass();
            }
        }
        
        private void UpdateDebuggerSettings()
        {
            Debug.Log("Updating CustomDebugger Settings...");
            Dictionary<string,string> logCategories = GetLogCategories();
            
            if (logCategories == null || logCategories.Count <= 0) //Mostrar mensaje de error, o volver a generar la clase
                return;
            
            if (_debuggerSettings.categories == null)
                _debuggerSettings.categories = new List<LogCategory>();

            if (_debuggerSettings.categories.Count < logCategories.Count)
            {
                foreach (var category in logCategories)
                    if (!_debuggerSettings.categories.Any(cat => string.Equals(cat.name, category.Key)))
                        _debuggerSettings.categories.Add(new LogCategory(){name = category.Key, enabled = true, customColor = newCategoryColor});
            }
            else if (_debuggerSettings.categories.Count > logCategories.Count)
            {
                UpdateLogCategoriesClass();
            }
        }

        void UpdateLogCategoriesClass()
        {
            Debug.Log("Updating LogCategories class...");
            // Ubicación de la clase LogCategories
            string path = "Assets/Resources/CustomDebugger/LogCategories.cs";

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine("namespace CDebugger");
                writer.WriteLine("{");
                writer.WriteLine("    public static class LogCategories");
                writer.WriteLine("    {");
                // Categorías personalizadas
                if (_debuggerSettings.categories != null && _debuggerSettings.categories.Count > 0)
                    foreach (var category in _debuggerSettings.categories)
                    {
                        writer.WriteLine($"        public static readonly string {category.name} = \"{category.name}\";");
                    }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void LoadSettings()
        {
            _debuggerSettings = AssetDatabase.LoadAssetAtPath<DebuggerSettings>(
                "Assets/Resources/CustomDebugger/CustomDebuggerSettings.asset");
            if (_debuggerSettings == null)
            {
                Debug.LogWarning("Custom Debugger settings not found! Creating new one...");
                CreateSettingsAsset();
            }
            _debuggerSettings.categories ??= new();
            UpdateDebuggerSettings();
        }

        private void CreateSettingsAsset()
        {
            _debuggerSettings = CreateInstance<DebuggerSettings>();
            string basePath = "Assets/Resources/CustomDebugger";
            var assetPath = EnsureValidAssetPath(basePath);
            
            AssetDatabase.CreateAsset(_debuggerSettings, assetPath);
            CreateDefaultLogCategories();
            UpdateLogCategoriesClass();
            UpdateDebuggerSettings();
            AssetDatabase.SaveAssets();
        }

        private void CreateDefaultLogCategories()
        {
            _debuggerSettings.categories ??= new();
            _debuggerSettings.categories.Add(new LogCategory() { name = "Default", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "Player", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "Input", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "Enemies", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "NPCs", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "UI", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "Map", enabled = true, customColor = newCategoryColor});
            _debuggerSettings.categories.Add(new LogCategory() { name = "Inventory", enabled = true, customColor = newCategoryColor});
        }

        private static string EnsureValidAssetPath(string basePath)
        {
            // Verificar si la carpeta 'Assets/Resources' existe, si no, crearla
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources"); // Crear la carpeta 'Resources' si no existe
            }

            // Verificar si la carpeta 'Assets/Resources/CustomDebugger' existe, si no, crearla
            if (!AssetDatabase.IsValidFolder(basePath))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "CustomDebugger"); // Crear la carpeta 'CustomDebugger' dentro de 'Resources'
            }

            // Ruta completa para el asset
            string assetPath = basePath + "/CustomDebuggerSettings.asset";
            return assetPath;
        }
        
        public static Dictionary<string, string> GetLogCategories()
        {
            Dictionary<string, string> categories = new Dictionary<string, string>();

            if (!File.Exists("Assets/Resources/CustomDebugger/LogCategories.cs"))
                return categories;
            
            Type logCategoriesType = Type.GetType("CDebugger.LogCategories, Assembly-CSharp");

            if (logCategoriesType == null)
            {
                Debug.LogWarning("LogCategories class not found.");
                return categories;
            }

            FieldInfo[] fields = logCategoriesType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string)) // Solo extraer strings
                {
                    string name = field.Name;
                    string value = (string)field.GetValue(null); // Obtener valor estático
                    categories[name] = value;
                }
            }

            return categories;
        }
    }
}