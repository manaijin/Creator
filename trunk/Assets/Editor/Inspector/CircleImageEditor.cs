using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleImage))]
public class CircleImageEditor : InspectorTool
{
    public override void OnInspectorGUI()
    {
        DrawPropertyField("m_Sprite");
        DrawPropertyField("m_Color");
        DrawPropertyField("m_Material");
        DrawPropertyField("Segements");
        DrawPropertyField("Fill");
        DrawPropertyField("Thickness");

        base.OnInspectorGUI();
    }
}
