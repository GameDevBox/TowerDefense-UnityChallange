using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameData))]
public class GameDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameData gameData = (GameData)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Data (Read Only)", EditorStyles.boldLabel);

        // Display private fields as read-only
        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.IntField("Score", gameData.Score);
        EditorGUILayout.IntField("Current Wave", gameData.CurrentWave);
        EditorGUILayout.IntField("Total Waves", gameData.TotalWaves);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset Game Data"))
        {
            gameData.ResetGame();
        }

        if (GUILayout.Button("Add 100 Score (Debug)"))
        {
            gameData.AddScore(100);
        }
    }
}