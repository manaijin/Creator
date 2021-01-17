using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsMenuExtension
{
    [MenuItem("Assets/生成UI模板代码")]
    static void CreateUICode()
    {
        UICodeGenerate.UICodeGeneratorWindow.CreateUICode();
    }
}
