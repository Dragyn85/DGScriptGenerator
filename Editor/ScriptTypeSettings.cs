using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class ScriptTypeSettings
{
    public string scriptType;
    public string templatePath;
    public string defaultFolder;
    public string fileNameModification;
    public List<string> subFolders = new List<string>();
}