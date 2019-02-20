using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AppManager))]
public class AppManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var appManager = (AppManager) target;

        if (GUILayout.Button("Record"))
        {
            appManager.ResetRecordingTimer();
            appManager.IsRecording = true;
        }

        if (GUILayout.Button("Replay"))
        {
            appManager.ResetReplayingTimer();
            appManager.IsReplaying = true;
        }

        if (GUILayout.Button("Stop"))
        {
            appManager.IsRecording = false;
            appManager.IsReplaying = false;
        }
    }
}