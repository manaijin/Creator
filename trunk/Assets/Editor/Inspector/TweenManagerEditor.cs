using Creator;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Creator.TweenManager;

[CustomEditor(typeof(TweenManager))]
public class TweenManagerEditor : InspectorTool
{
    GUIContent[] m_EventTypes;
    GUIContent m_AddButonContent;

    void OnEnable()
    {
        string[] eventNames = Enum.GetNames(typeof(TweenType));
        m_EventTypes = new GUIContent[eventNames.Length];
        for (int i = 0; i < eventNames.Length; ++i)
        {
            m_EventTypes[i] = new GUIContent(eventNames[i]);
        }
        m_AddButonContent = EditorGUIUtility.TrTextContent("Add Tween Type");
    }

    void ShowAddTriggermenu()
    {
        GenericMenu menu = new GenericMenu();
        for (int i = 0; i < m_EventTypes.Length; ++i)
        {
            menu.AddItem(m_EventTypes[i], false, OnAddNewSelected, i);
        }
        menu.ShowAsContext();
        Event.current.Use();
    }

    private void OnAddNewSelected(object index)
    {
        Type comp = typeof(TweenBase);
        switch (index)
        {
            case (int)TweenType.PositionTween:
                comp = ((TweenManager)target).gameObject.AddComponent<PositionTween>().GetType();
                break;
            case (int)TweenType.RotationTween:

                break;
            case (int)TweenType.ScaleTween:
                comp = ((TweenManager)target).gameObject.AddComponent<ScaleTween>().GetType();
                break;
            case (int)TweenType.ColorTween:
                comp = ((TweenManager)target).gameObject.AddComponent<ColorTween>().GetType();
                break;
        }
        OpenComponent(comp.GetType().Name);
    }

    public override void OnInspectorGUI()
    {
        DrawPropertyField("duration");
        DrawPropertyField("loopNum");
        DrawPropertyField("ease");
        DrawPropertyField("loopType");
        DrawPropertyField("endAction");
        if (((TweenManager)target).endAction == PlayEndDo.Cache)
        {
            DrawPropertyField("poolName");
        }

        Rect rect = GUILayoutUtility.GetRect(m_AddButonContent, GUI.skin.button);
        rect.x += (rect.width - 200) / 2;
        rect.width = 200;
        if (GUI.Button(rect, m_AddButonContent))
        {
            ShowAddTriggermenu();
        }
        base.OnInspectorGUI();
    }
}
