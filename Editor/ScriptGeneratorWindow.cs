using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DragynGames.Editor.ScriptGeneration
{
    public class ScriptGeneratorWindow : EditorWindow
    {
        private static ScriptGeneratorWindow window;
        
        private ScriptGenerationConfig lastConfig;
        private ScriptGenerationConfig config;
        public ScriptGenerator scriptGenerator;

        private string[] scriptTypes;
        private string scriptName = "NewScript";
        private int selectedScriptType = 0;
        private int selectedSubFolderIndex = 0;
        private string newSubFolder = "";
        private ConfigHandler configHandler;
        private bool showExtraSettings = false;
        private int selectedExtraSettings = 0;
        

        [MenuItem("DG Tools/Script Generator")]
        public static void ShowWindow()
        {
            window = GetWindow<ScriptGeneratorWindow>("Script Generator");
        }

        private void OnEnable()
        {
            configHandler = new ConfigHandler();
            config = configHandler.GetConfig();
            selectedScriptType = EditorPrefs.GetInt("SelectedScriptType", 0);
        }

        private void OnGUI()
        {
            if (ShowConfigFileSelection()) return;

            ShowScriptSelection();

            ShowSubfolderSelection();

            ExtraSettings();

            EditorGUILayout.Space(10);

            
        }

        private void ExtraSettings()
        {
            showExtraSettings = EditorGUILayout.Foldout(showExtraSettings, "Extra settings");
            if (showExtraSettings)
            {
                ShowExtraSettings();
            }
        }

        private bool ShowConfigFileSelection()
        {
            config = (ScriptGenerationConfig) EditorGUILayout.ObjectField("Config", config,
                typeof(ScriptGenerationConfig), false);
            if(lastConfig != null && lastConfig != config)
            {
                ScriptGeneratorProjectSettings.SetConfigPath(AssetDatabase.GetAssetPath(config));
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

                return true;
            }

            return false;
        }

        private void ShowScriptSelection()
        {
            EditorGUILayout.LabelField("Select Script Type:");

            string[] scriptTypes = new string[config.scriptTypeAndFolders.Length];
            for (int i = 0; i < config.scriptTypeAndFolders.Length; i++)
            {
                scriptTypes[i] = config.scriptTypeAndFolders[i].scriptType;
            }
            
            selectedScriptType = EditorGUILayout.Popup("Script Type", selectedScriptType, scriptTypes);
            EditorPrefs.SetInt("SelectedScriptType", selectedScriptType); // Save the selection in EditorPrefs
            EditorGUILayout.Space();
            
            scriptName = EditorGUILayout.TextField("Script Name", scriptName);
            this.scriptTypes = scriptTypes;
        }

        private void ShowSubfolderSelection()
        {
            List<string> subFolders = config.GetSubFolders(scriptTypes[selectedScriptType]);
            if (subFolders.Count == 0)
            {
                subFolders.Add("None");
                selectedSubFolderIndex = 0;
            }

            selectedSubFolderIndex = EditorGUILayout.Popup("Subfolder", selectedSubFolderIndex, subFolders.ToArray());
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Script", GUILayout.Height(40)))
            {
                CreateScript(scriptTypes[selectedScriptType], subFolders[selectedSubFolderIndex]);
            }
        }

        private void ShowExtraSettings()
        {
            selectedExtraSettings = GUILayout.SelectionGrid(selectedExtraSettings, new string[] {"Sub folder", "Namespace"}, 3);
            switch (selectedExtraSettings)
            {
                case 0:
                    ShowSubfolderCreation();
                    break;
                case 1:
                    ShowNamespaceSettings();
                    break;
            }
            
            
        }

        private void ShowNamespaceSettings()
        {
            EditorGUILayout.LabelField("Namespace");
        }

        private void ShowSubfolderCreation()
        {
            EditorGUILayout.LabelField("New subfolder");
            EditorGUILayout.BeginHorizontal();
            // Input field and button to add a new subfolder

            newSubFolder = EditorGUILayout.TextField(newSubFolder);
            if (GUILayout.Button("Create Subfolder"))
            {
                CreateSubfolder(newSubFolder);
                newSubFolder = "";
            }

            EditorGUILayout.EndHorizontal();
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