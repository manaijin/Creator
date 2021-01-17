using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyPath
{
    public static void CopyNodePath()
    {
        if (Selection.activeGameObject)
        {
            var list = new List<Transform>();
            var path = string.Empty;
            GetParent(Selection.activeGameObject.transform, ref list, ref path);
            CopyString(path);
        }
    }

    private static void GetParent(Transform tr, ref List<Transform> result, ref string path)
    {
        result.Add(tr);
        path = tr.name + path;
        path = '/' + path;
        if (tr.parent != null)
        {
            GetParent(tr.parent, ref result, ref path);
        }
        else
        {
            path = path.Substring(1);
        }
    }

    private static void CopyString(string str)
    {
        TextEditor te = new TextEditor();
        te.text = str;
        te.SelectAll();
        te.Copy();
    }
}
