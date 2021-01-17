using Creator;
using UnityEditor;
using static Creator.PositionTween;

[CustomEditor(typeof(PositionTween))]
public class PositionTweenEditor : InspectorTool
{
    private PositionTween testA;
    public override void OnInspectorGUI()
    {
        DrawPropertyField("type");

        testA = (PositionTween)target;
        
        if (testA.type == MoveType.FromPositionToTarget || testA.type == MoveType.FromTargetToPosition)
        {
            DrawPropertyField("target");
        }

        if (testA.type == MoveType.OffsetToPosition || testA.type == MoveType.PositionToOffset)
        {
            DrawPropertyField("offset");
        }
        base.OnInspectorGUI();
    }
}
