
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(SimpleScreenshot))]
public class SimpleScreenshotEditor : Editor
{
    private SerializedProperty screenshotKeyProp;
    private SerializedProperty folderNameProp;
    private SerializedProperty superSizeProp;
    private SerializedProperty includeTimestampProp;
    private SerializedProperty transparentBackgroundProp;
    private SerializedProperty customSavePathProp;
    private SerializedProperty useCustomPathProp;

    private void OnEnable()
    {
        screenshotKeyProp = serializedObject.FindProperty("screenshotKey");
        folderNameProp = serializedObject.FindProperty("folderName");
        superSizeProp = serializedObject.FindProperty("superSize");
        includeTimestampProp = serializedObject.FindProperty("includeTimestamp");
        transparentBackgroundProp = serializedObject.FindProperty("transparentBackground");
        customSavePathProp = serializedObject.FindProperty("customSavePath");
        useCustomPathProp = serializedObject.FindProperty("useCustomPath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SimpleScreenshot screenshot = (SimpleScreenshot)target;

        EditorGUILayout.PropertyField(screenshotKeyProp);
        EditorGUILayout.PropertyField(folderNameProp);
        EditorGUILayout.PropertyField(superSizeProp);
        EditorGUILayout.PropertyField(includeTimestampProp);
        EditorGUILayout.PropertyField(transparentBackgroundProp);

        EditorGUILayout.Space();
        GUILayout.Label("Save Location Management", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(useCustomPathProp);
        EditorGUILayout.PropertyField(customSavePathProp);

        EditorGUILayout.LabelField("Current Save Path:", screenshot.GetDisplayPath());

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Choose Save Folder"))
        {
            string currentPath = screenshot.GetSaveFolderPath();
            string selectedPath = EditorUtility.SaveFolderPanel("Select Screenshot Save Location",
                currentPath, "");

            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Convert to relative path if within Assets folder
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    customSavePathProp.stringValue = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    customSavePathProp.stringValue = selectedPath;
                }
                useCustomPathProp.boolValue = true;
            }
        }

        if (GUILayout.Button("Use Assets Folder"))
        {
            useCustomPathProp.boolValue = false;
            customSavePathProp.stringValue = "";
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Assets/Screenshots"))
        {
            useCustomPathProp.boolValue = true;
            customSavePathProp.stringValue = "Assets/Screenshots";
        }

        if (GUILayout.Button("Project Root"))
        {
            useCustomPathProp.boolValue = true;
            customSavePathProp.stringValue = Application.dataPath + "/../Screenshots";
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Test button
        if (GUILayout.Button("Test Camera Screenshot"))
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                screenshot.TakeCameraScreenshot(mainCamera);
            }
            else
            {
                Debug.LogError("No main camera found in scene!");
            }
        }

        if (GUILayout.Button("Open Save Folder"))
        {
            string folderPath = screenshot.GetSaveFolderPath();
            if (Directory.Exists(folderPath))
            {
                EditorUtility.RevealInFinder(folderPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Folder Not Found",
                    $"The folder does not exist yet:\n{folderPath}\n\nTake a screenshot first to create it.", "OK");
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label("Instructions:", EditorStyles.helpBox);
        GUILayout.Label("• Press F12 to take screenshot", EditorStyles.miniLabel);
        GUILayout.Label("• Use 'Test Camera Screenshot' for transparent backgrounds", EditorStyles.miniLabel);
        GUILayout.Label("• Screenshots save to: " + screenshot.GetDisplayPath(), EditorStyles.miniLabel);

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
#endif