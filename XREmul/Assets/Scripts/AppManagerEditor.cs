using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AppManager))]
public class AppManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var appManager = (AppManager) target;

        if (GUILayout.Button("Replay"))
        {
            appManager.ResetReplayingTimer();
            appManager.IsReplaying = true;
        }
    }
}