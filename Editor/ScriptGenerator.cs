using System.IO;
using DragynGames.Editor.ScriptGeneration;
using UnityEditor;
using UnityEngine;

public class ScriptGenerator
{
    private const string packageTemplatePath = "Packages/com.dragyngames.dgscriptgenerator/Editor/Templates/";
    public void CreateScript(string scriptType, string subFolder, string scriptName, ScriptGenerationConfig config)
    {
        
        
        string defaultFolder = config.GetDefaultFolder(scriptType);
        string scriptFileName = ReplacePlaceHolderInFileName(scriptName, config.GetFileNameModification(scriptType));
        
        if (DoesClassNameExist(scriptName,scriptFileName))
        {
            EditorUtility.DisplayDialog("Error", $"A script with the name {scriptName}  or already exists.", "OK");
            return;
        }
        
        string folderPath = Path.Combine(ScriptGeneratorProjectSettings.GetBaseFolderPath(), defaultFolder, subFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string scriptPath = Path.Combine(folderPath, $"{scriptFileName}.cs");


        string packageTemplateFolder = Path.Combine(config.GetTemplateFilePath(scriptType));
        
        //string templatePath = $"Assets/DGDotsTools/Editor/Templates/{config.GetTemplateName(scriptType)}Template.txt";
        string templatePath = packageTemplateFolder;
        if (!File.Exists(templatePath))
        {
            Debug.LogError($"Template not found at {templatePath}");
            return;
        }

        string scriptTemplate = File.ReadAllText(templatePath);

        IConvertTemplate converter = GetConverterType(scriptType);

        if (converter == null)
        {
            Debug.LogError($"Converter not found for {scriptType}");
            return;
        }

        string finalScriptContent = converter.Convert(scriptTemplate, scriptName);


        File.WriteAllText(scriptPath, finalScriptContent);
        AssetDatabase.Refresh();

        Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
        int option = EditorUtility.DisplayDialogComplex("Script Created", $"Script created at {scriptPath}",
            "Open File", "Show in Project", "Close");

        switch (option)
        {
            case 0: // "Open File"
                AssetDatabase.OpenAsset(scriptAsset);
                break;
            case 1: // "Show in Project"
                EditorGUIUtility.PingObject(scriptAsset);
                break;
            case 2: // "Close"
                // Do nothing, just close the dialog
                break;
        }
    }

    private IConvertTemplate GetConverterType(string scriptType)
    {
        switch (scriptType)
        {
            case "Baker":
                return new TemplateConverter();
            default:
                return new TemplateConverter();
        }
    }

    private string ReplacePlaceHolderInFileName(string scriptName, string fileNameWithPlaceHolder)
    {
        return fileNameWithPlaceHolder.Replace(ScriptGenerationConfig.CLASS_NAME_PLACEHOLDER, scriptName);
    }
    
    private bool DoesClassNameExist(string className, string fileName)
    {
        // Check if a script with the same class name already exists
        string[] guids = AssetDatabase.FindAssets($"{fileName} t:script");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string scriptContent = File.ReadAllText(path);
            if (scriptContent.Contains($"class {className}") || scriptContent.Contains($"struct {className}"))
            {
                return true;
            }
        }

        // Get all assemblies loaded in the current app domain
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        // Filter the assemblies to only include those relevant to your project
        foreach (var assembly in assemblies)
        {
            // Skip assemblies that are not part of the project's compiled code (like UnityEngine, third-party libraries, etc.)
            if (assembly.FullName.StartsWith("Unity") || assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("mscorlib"))
            {
                continue;
            }

            // Check if the class exists in the assembly
            var existingClass = assembly.GetType(className);
            if (existingClass != null)
            {
                return true;
            }
        }

        return false;
    }
}