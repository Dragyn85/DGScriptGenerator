using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragynGames.Editor.ScriptGeneration
{
    [CreateAssetMenu(fileName = "ScriptGenerationConfig", menuName = "Script Generation/Config", order = 1)]
    public class ScriptGenerationConfig : ScriptableObject
    {
        public const string CLASS_NAME_PLACEHOLDER = "#SCRIPTNAME#";
        public const string NAMESPACE_PLACEHOLDER = "#NAMESPACE#";
        public const string NAMESPACE_END_PLACEHOLDER = "#NAMESPACEEND#";
        
        public string baseFolder = "Assets/Scripts";
        public ScriptTypeSettings[] scriptTypeAndFolders = new ScriptTypeSettings[]
        {
            new ScriptTypeSettings {scriptType = "ISystem", TemplateName = "ISystem", defaultFolder = "Systems",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "IJobEntity",TemplateName = "IJobEntity", defaultFolder = "JobEntities",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "IComponentData",TemplateName = "IComponentData", defaultFolder = "ComponentDatas",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "Baker",TemplateName = "Baker", defaultFolder = "Authoring",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}Authoring"},
            new ScriptTypeSettings {scriptType = "MonoBehaviour",TemplateName = "MonoBehaviour", defaultFolder = "MonoBehaviours",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
        };


        public List<string> GetSubFolders(string scriptType)
        {
            foreach (ScriptTypeSettings scriptTypeAndFolder in scriptTypeAndFolders)
            {
                if (scriptTypeAndFolder.scriptType == scriptType)
                {
                    return scriptTypeAndFolder.subFolders;
                }
            }

            return new List<string>();
        }

        public string GetDefaultFolder(string scriptType)
        {
            foreach (ScriptTypeSettings scriptTypeAndFolder in scriptTypeAndFolders)
            {
                if (scriptTypeAndFolder.scriptType == scriptType)
                {
                    return scriptTypeAndFolder.defaultFolder;
                }
            }

            return "";
        }
        
        public string GetTemplateName(string scriptType)
        {
            foreach (ScriptTypeSettings scriptTypeAndFolder in scriptTypeAndFolders)
            {
                if (scriptTypeAndFolder.scriptType == scriptType)
                {
                    return scriptTypeAndFolder.TemplateName;
                }
            }

            return "";
        }
        public string GetFileNameModification(string scriptType)
        {
            foreach (ScriptTypeSettings scriptTypeAndFolder in scriptTypeAndFolders)
            {
                if (scriptTypeAndFolder.scriptType == scriptType)
                {
                    return scriptTypeAndFolder.fileNameModification;
                }
            }

            return "";
        }
    }
}

[Serializable]
public class ScriptTypeSettings
{
    public string scriptType;
    public string TemplateName;
    public string defaultFolder;
    public string fileNameModification;
    public List<string> subFolders = new List<string>();
}