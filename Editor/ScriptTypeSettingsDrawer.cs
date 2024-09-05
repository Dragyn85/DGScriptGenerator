using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptTypeSettings))]
public class ScriptTypeSettingsDrawer : PropertyDrawer
{
    
    private static Dictionary<string, bool> objectFoldouts = new();
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string uniqueId = property.propertyPath;
        if(!objectFoldouts.ContainsKey(uniqueId))
        {
            objectFoldouts.Add(uniqueId, false);
        }
        
        float currentY = position.y;
        
        objectFoldouts[uniqueId] = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), objectFoldouts[uniqueId], label, true);
        
        if(objectFoldouts[uniqueId] == false)
            return;
        
        currentY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        EditorGUI.BeginProperty(position, label, property);
        
        // Reflectively get the target object
        object typeSettingsObject = GetTargetObjectOfProperty(property);

        if (typeSettingsObject != null)
        {
            // Get the fields of the class using reflection
            FieldInfo[] fields = typeSettingsObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            

            // Loop through each field and create a PropertyField for each
            foreach (FieldInfo field in fields)
            {
                // Get the SerializedProperty corresponding to the field
                SerializedProperty fieldProp = property.FindPropertyRelative(field.Name);
                if (fieldProp != null)
                {
                    // Use EditorGUI.GetPropertyHeight to calculate proper height for this field
                    float propertyHeight = EditorGUI.GetPropertyHeight(fieldProp, true);
                    Rect fieldRect = new Rect(position.x, currentY, position.width, propertyHeight);
                    
                    // Draw the field
                    EditorGUI.PropertyField(fieldRect, fieldProp, new GUIContent(ObjectNames.NicifyVariableName(field.Name)), true);
                    
                    // Increment Y position for the next field
                    currentY += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            EditorGUI.EndProperty();

            // Now add the custom button for selecting a file
            Rect SelectTempateRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(SelectTempateRect, "Select template File"))
            {
                string selectedPath = EditorUtility.OpenFilePanel("Select template File", "", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Use reflection to set the 'filePath' field
                    FieldInfo filePathField = typeSettingsObject.GetType().GetField("templatePath", BindingFlags.Public | BindingFlags.Instance);
                    if (filePathField != null)
                    {
                        //Get the relative path from the selected path
                        int indexToStartPath = Application.dataPath.LastIndexOf('/') + 1;
                        string relativePath = selectedPath.Substring(indexToStartPath);
                        
                        filePathField.SetValue(typeSettingsObject, relativePath);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            
            currentY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            Rect selectSubFolderPath = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(selectSubFolderPath, "Add subfolder"))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select folder to add", "", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Use reflection to set the 'filePath' field
                    FieldInfo subFoldersFieldInfo = typeSettingsObject.GetType().GetField("subFolders", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo baseFolderField = typeSettingsObject.GetType().GetField("defaultFolder", BindingFlags.Public | BindingFlags.Instance);
                    if (subFoldersFieldInfo != null && baseFolderField != null)
                    
                    {
                        //Get the relative path from the selected path
                        int indexToStartPath = Application.dataPath.Length-6;
                        string relativePath = selectedPath.Substring(indexToStartPath);
                        
                        string basePath = ScriptGeneratorProjectSettings.GetBaseFolderPath();
                        string subPath = (string) baseFolderField.GetValue(typeSettingsObject);
                        if (!relativePath.StartsWith(basePath+subPath))
                        {
                            Debug.LogError("Selected folder is not a subfolder of the base folder");
                            return;
                        }
                        string relativeRelativePath = relativePath.Substring(basePath.Length+1+subPath.Length+1);

                        List<string> subfolders = (List<string>) subFoldersFieldInfo.GetValue(typeSettingsObject);
                        if (subfolders.Contains(relativeRelativePath))
                        {
                            Debug.LogWarning("Subfolder already exists");
                            return;
                        }
                        subfolders.Add(relativeRelativePath);
                        //subFoldersFieldInfo.SetValue(typeSettingsObject, subfolders);
                        
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            
        }

        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Reflectively get the target object
        object typeSettingsObject = GetTargetObjectOfProperty(property);
        float totalHeight = 0;
    
        if (typeSettingsObject != null)
        {
            string uniqueId = property.propertyPath;
            objectFoldouts.TryGetValue(uniqueId, out bool Foldout);
            if(Foldout == false)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            // Get the fields of the class using reflection
            FieldInfo[] fields = typeSettingsObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Loop through each field and calculate the total height
            foreach (FieldInfo field in fields)
            {
                SerializedProperty fieldProp = property.FindPropertyRelative(field.Name);
                if (fieldProp != null)
                {
                    // Use EditorGUI.GetPropertyHeight to get the correct height for each field
                    totalHeight += EditorGUI.GetPropertyHeight(fieldProp, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            // Add height for the button
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        
        return totalHeight;
    }

    // Helper method to get the target object from the SerializedProperty
    private object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        string[] elements = path.Split('.');

        foreach (string element in elements)
        {
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }

        return obj;
    }

    private object GetValue_Imp(object source, string name)
    {
        if (source == null) return null;

        var type = source.GetType();
        var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (f == null)
        {
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) return null;
            return p.GetValue(source, null);
        }

        return f.GetValue(source);
    }

    private object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        var enm = enumerable.GetEnumerator();

        // Move to the indexed element
        while (index-- >= 0)
            enm.MoveNext();

        return enm.Current;
    }
}