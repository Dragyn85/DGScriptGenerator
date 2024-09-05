using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DragynGames.Editor.ScriptGeneration
{
    public class ScriptGeneratorWindow : EditorWindow
    {
        private ScriptGenerationConfig lastConfig;
        private ScriptGenerationConfig config;
        public ScriptGenerator scriptGenerator;

        private string scriptName = "NewScript";
        private int selectedScriptType = 0;
        private int selectedSubFolderIndex = 0;
        private string newSubFolder = "";

        private ConfigHandler configHandler;

        [MenuItem("DG Tools/Script Generator")]
        public static void ShowWindow()
        {
            GetWindow<ScriptGeneratorWindow>("Script Generator");
        }

        private void OnEnable()
        {
            configHandler = new ConfigHandler();
            config = configHandler.GetConfig();
            selectedScriptType = EditorPrefs.GetInt("SelectedScriptType", 0);
        }

        private void OnGUI()
        {
            config = (ScriptGenerationConfig) EditorGUILayout.ObjectField("Config", config,
                typeof(ScriptGenerationConfig), false);
            if(lastConfig != null && lastConfig != config)
            {
                ProjectSettingsPath.SetConfigPath(AssetDatabase.GetAssetPath(config));
            }
            
            lastConfig = config;

            if (config == null)
            {
                EditorGUILayout.HelpBox("Please create or assign a ScriptGenerationConfig.", MessageType.Warning);
                if (GUILayout.Button("Create New Config"))
                {
                    config = configHandler.CreateConfig();
                }

                if (GUILayout.Button("Load Existing Config"))
                {
                    config = configHandler.LoadExistingConfig();
                }

                return;
            }
            /*
            //Grid layout script selection
            EditorGUILayout.LabelField("Select Script Type:");
            
            string[] scriptTypes = new string[config.scriptTypeAndFolders.Length];
            for (int i = 0; i < config.scriptTypeAndFolders.Length; i++)
            {
                scriptTypes[i] = config.scriptTypeAndFolders[i].scriptType;
            }

            selectedScriptType = GUILayout.SelectionGrid(selectedScriptType, scriptTypes, 2);
            EditorPrefs.SetInt("SelectedScriptType", selectedScriptType);
            EditorGUILayout.Space();
            */
            
            EditorGUILayout.LabelField("Select Script Type:");

            string[] scriptTypes = new string[config.scriptTypeAndFolders.Length];
            for (int i = 0; i < config.scriptTypeAndFolders.Length; i++)
            {
                scriptTypes[i] = config.scriptTypeAndFolders[i].scriptType;
            }

            // Display the dropdown for selecting the script type
            selectedScriptType = EditorGUILayout.Popup("Script Type", selectedScriptType, scriptTypes);
            EditorPrefs.SetInt("SelectedScriptType", selectedScriptType); // Save the selection in EditorPrefs
            EditorGUILayout.Space();
            
            scriptName = EditorGUILayout.TextField("Script Name", scriptName);

            // Display dropdown for selecting subfolder
            List<string> subFolders = config.GetSubFolders(scriptTypes[selectedScriptType]);
            if (subFolders.Count == 0)
            {
                subFolders.Add("None");
            }

            selectedSubFolderIndex = EditorGUILayout.Popup("Subfolder", selectedSubFolderIndex, subFolders.ToArray());

            // Input field and button to add a new subfolder
            newSubFolder = EditorGUILayout.TextField("New Subfolder", newSubFolder);
            if (GUILayout.Button("Create Subfolder"))
            {
                CreateSubfolder(newSubFolder);
                newSubFolder = "";
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Script"))
            {
                CreateScript(scriptTypes[selectedScriptType], subFolders[selectedSubFolderIndex]);
            }
        }

        // Adds a new subfolder to the list and saves it
        private void CreateSubfolder(string subFolderName)
        {
            if (string.IsNullOrEmpty(subFolderName))
            {
                EditorUtility.DisplayDialog("Error", "Subfolder name cannot be empty", "OK");
                return;
            }

            List<string> subFolders = config.scriptTypeAndFolders[selectedScriptType].subFolders;
            if (!subFolders.Contains(subFolderName))
            {
                subFolders.Add(subFolderName);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                selectedSubFolderIndex = subFolders.Count - 1; // Select the new subfolder in the dropdown
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Subfolder already exists", "OK");
            }
        }

        private void CreateScript(string scriptType, string subFolder)
        {
            if(subFolder == "None")
            {
                subFolder = "";
            }
            
            scriptGenerator = new ScriptGenerator();
            scriptGenerator.CreateScript(scriptType, subFolder, scriptName, config);
        }
    }
}