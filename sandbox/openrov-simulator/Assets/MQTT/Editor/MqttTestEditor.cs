using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MqttTest))]
public class MqttTestEditor : Editor
{
    protected string message = "\"20:-20\"";

    public override void OnInspectorGUI()
    {
        var mqttTest = target as MqttTest;

        DrawDefaultInspector();

        EditorGUILayout.BeginVertical();

        message = EditorGUILayout.TextField(message);

        if(GUILayout.Button("Send Command"))
        {
            mqttTest.PublishSomething(message);
        }

        EditorGUILayout.EndVertical();
    }
}
