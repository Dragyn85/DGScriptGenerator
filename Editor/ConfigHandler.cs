using UnityEditor;
using UnityEngine;

namespace DragynGames.Editor.ScriptGeneration
{
    public class ConfigHandler
    {
        private const string CONFIG_ASSET_PATHE_KEY = "ScriptGenerationConfigPath";
        
        public ScriptGenerationConfig GetConfig()
        {
            ScriptGenerationConfig config = null;
            string savedConfigPath = ProjectSettingsPath.GetConfigPath();
            if (!string.IsNullOrEmpty(savedConfigPath))
            {
                config = AssetDatabase.LoadAssetAtPath<ScriptGenerationConfig>(savedConfigPath);
            }

            return config;
        }
        
        public ScriptGenerationConfig CreateConfig()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Config", "ScriptGenerationConfig", "asset",
                "Please enter a file name to save the config.");
            if (!string.IsNullOrEmpty(path))
            {
                ScriptGenerationConfig config = ScriptableObject.CreateInstance<ScriptGenerationConfig>();
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Config Created", "New ScriptGenerationConfig asset created at: " + path,
                    "OK");

                // Save path to EditorPrefs
                ProjectSettingsPath.SetConfigPath(path);
                return config;
            }

            return null;
        }
        
        public ScriptGenerationConfig LoadExistingConfig()
        {
            string path = EditorUtility.OpenFilePanel("Select ScriptGenerationConfig", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                // Convert absolute path to relative path (required by AssetDatabase)
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                ScriptGenerationConfig config = AssetDatabase.LoadAssetAtPath<ScriptGenerationConfig>(relativePath);

                if (config != null)
                {
                    // Save the path in EditorPrefs
                    ProjectSettingsPath.SetConfigPath(relativePath);
                    EditorUtility.DisplayDialog("Config Loaded", "Config loaded from: " + relativePath, "OK");
                    return config;
                }
            }
            EditorUtility.DisplayDialog("Error", "Could not load config from the selected file.", "OK");
            return null;
        }
    }
    
    
}