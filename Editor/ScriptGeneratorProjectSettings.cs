using UnityEditor;
using UnityEngine;
using System.IO;

public static class ScriptGeneratorProjectSettings
{
    private static string settingsFilePath = "ProjectSettings/ScriptGenerationSettings.json";

    // This will hold the path to the config file
    [System.Serializable]
    public class ScriptGenerationSettings
    {
        public string configPath;
        public string baseFolderPath = "Assets/Scripts";
    }

    private static ScriptGenerationSettings settings;

    // Expose the settings in the Project Settings window
    [SettingsProvider]
    public static SettingsProvider CreateScriptGeneratorSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Script Generator", SettingsScope.Project)
        {
            label = "Script Generator",
            guiHandler = (searchContext) =>
            {
                LoadSettings();

                // Display the current path
                EditorGUILayout.LabelField("Config File Path", EditorStyles.boldLabel);
                settings.configPath = EditorGUILayout.TextField("Config Path", settings.configPath);

                // Button to select a new config path
                if (GUILayout.Button("Browse for Config"))
                {
                    string selectedPath = EditorUtility.OpenFilePanel("Select Config File", "Assets", "asset");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        // Convert absolute path to relative path
                        string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        settings.configPath = relativePath;
                        SaveSettings();
                    }
                }
                
                EditorGUILayout.LabelField("Base script folder", EditorStyles.boldLabel);
                settings.baseFolderPath = EditorGUILayout.TextField("path", settings.baseFolderPath);

                // Button to select a new config path
                if (GUILayout.Button("Browse for folder"))
                {
                    string selectedPath = EditorUtility.OpenFolderPanel("Select base folder", "Assets", "asset");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        // Convert absolute path to relative path
                        string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        settings.baseFolderPath = relativePath;
                        SaveSettings();
                    }
                }

                // Save the settings if any changes occur
                if (GUI.changed)
                {
                    SaveSettings();
                }
            },

            // Define keywords to help with searching in the settings window
            keywords = new System.Collections.Generic.HashSet<string>(new[] { "Script", "Generator", "Config", "Path" })
        };

        return provider;
    }

    // Load the settings from the JSON file in ProjectSettings
    private static void LoadSettings()
    {
        if (settings == null)
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                settings = JsonUtility.FromJson<ScriptGenerationSettings>(json);
            }
            else
            {
                // If no settings file exists, create a new one
                settings = new ScriptGenerationSettings();
                SaveSettings();
            }
        }
    }

    // Save the settings to the JSON file in ProjectSettings
    private static void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public static void SetConfigPath(string path)
    {
        // Ensure settings are loaded before modifying
        LoadSettings();

        // Update the config path
        settings.configPath = path;

        // Save the updated settings
        SaveSettings();
    }
    public static void SetBaseFolderPath(string path)
    {
        // Ensure settings are loaded before modifying
        LoadSettings();

        // Update the config path
        settings.baseFolderPath = path;

        // Save the updated settings
        SaveSettings();
    }
    
    // Get the config path (public method for accessing the stored path)
    public static string GetConfigPath()
    {
        LoadSettings();
        return settings.configPath;
    }
    public static string GetBaseFolderPath()
    {
        LoadSettings();
        return settings.baseFolderPath;
    }
}
