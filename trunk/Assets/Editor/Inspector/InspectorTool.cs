using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;

public class InspectorTool : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    protected void DrawPropertyField(string filedName)
    {
        DrawPropertyField(filedName, true, true);
    }

    protected void DrawPropertyField(string fieldName, bool isValid, bool isValidWarning)
    {
        WrapWithValidationColor(() =>
        {
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            EditorGUILayout.PropertyField(property);
        }, isValid, isValidWarning);
    }

    protected void WrapWithValidationColor(System.Action method, bool isValid, bool isValidWarning)
    {
        Color colorBackup = GUI.color;
        if (isValid == false)
        {
            GUI.color = Color.red;
        }
        else if (isValidWarning == false)
        {
            GUI.color = Color.yellow;
        }
        method.Invoke();
        GUI.color = colorBackup;
    }

    protected void OpenComponent(string name)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(type);
        FieldInfo info = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
        ActiveEditorTracker tracker = info.GetValue(window) as ActiveEditorTracker;

        for (int i = 0; i < tracker.activeEditors.Length; i++)
        {
            //可以通过名子单独判断组件展开或不展开
            if (tracker.activeEditors[i].target.GetType().Name == name)
            {
                //这里1就是展开，0就是合起来
                tracker.SetVisible(i, 1);
            }
        }
    }
}