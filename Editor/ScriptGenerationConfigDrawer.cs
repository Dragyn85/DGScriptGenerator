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
            if (GUILayout.Button("Change base folder"))
            {
                // Open folder selection dialog
                string path = EditorUtility.OpenFolderPanel("Select folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    string relativePath = "Assets" + path.Substring(Application.dataPath.Length + 1);
                    
                    //set projectsetings baseFolderPath to releativePath
                    ScriptGeneratorProjectSettings.SetBaseFolderPath(relativePath);
                    EditorUtility.DisplayDialog("Base folder set", $"Base folder set to {relativePath} in project settings", "OK");
                }
            }
        }
    }
}