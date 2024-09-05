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
        
        private const string packageTemplateFolder = "Packages/com.dragyngames.dgscriptgenerator/Editor/Templates/";
        
        public ScriptTypeSettings[] scriptTypeAndFolders = new ScriptTypeSettings[]
        {
            new ScriptTypeSettings {scriptType = "ISystem", templatePath = packageTemplateFolder+"ISystemTemplate.txt", defaultFolder = "Systems",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "IJobEntity",templatePath = packageTemplateFolder+"IJobEntityTemplate.txt", defaultFolder = "JobEntities",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "IComponentData",templatePath = packageTemplateFolder+"IComponentDataTemplate.txt", defaultFolder = "ComponentDatas",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
            new ScriptTypeSettings {scriptType = "Baker",templatePath = packageTemplateFolder+"BakerTemplate.txt", defaultFolder = "Authoring",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}Authoring"},
            new ScriptTypeSettings {scriptType = "MonoBehaviour",templatePath = packageTemplateFolder+"MonoBehaviourTemplate.txt", defaultFolder = "MonoBehaviours",fileNameModification = $"{CLASS_NAME_PLACEHOLDER}"},
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
        
        public string GetTemplateFilePath(string scriptType)
        {
            foreach (ScriptTypeSettings scriptTypeAndFolder in scriptTypeAndFolders)
            {
                if (scriptTypeAndFolder.scriptType == scriptType)
                {
                    return scriptTypeAndFolder.templatePath;
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