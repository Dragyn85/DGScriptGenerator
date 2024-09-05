using UnityEditor;
using UnityEngine;


namespace DragynGames.Editor.ScriptGeneration
{


    [CustomEditor(typeof(ScriptGenerationConfig))]
    public class ScriptGenerationConfigDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ScriptGenerationConfig config = (ScriptGenerationConfig) target;
            if (GUILayout.Button("Get folder path"))
            {
                // Open folder selection dialog
                string path = EditorUtility.OpenFolderPanel("Select folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    string baseFolder = config.baseFolder;
                    
                    //remove all text before baseFolder
                    int index = path.IndexOf(baseFolder);
                    if (index != -1)
                    {
                        path = path.Substring(index);
                        //remove baseFolder from path
                        path = path.Replace(baseFolder+"/", "");
                        
                        //copy path to clipboard
                        
                        EditorGUIUtility.systemCopyBuffer = path;
                        Debug.Log("Copied path to clipboard: " + path);
                    }
                    else
                    {
                        Debug.LogError("Selected folder is not in the base folder");
                    }
                }
            }
        }
    }
}