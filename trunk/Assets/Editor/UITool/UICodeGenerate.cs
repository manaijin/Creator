using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 生成UI模板代码
/// </summary>
public class UICodeGenerate : Editor
{
    public class UICodeGeneratorWindow : EditorWindow
    {
        private static UICodeParam param;
        private static string uiRootPath_tag = "UI Prefab根目录";
        private static string codeRootPath_tag = "UI代码根目录";
        private static string types_tag = "前缀名称";
        private static string separator_tag = "前后缀分隔符";
        private static string nameSpace_tag = "命名空间";
        private static string templatePath_tag = "代码模板路径";

        [MenuItem("Tools/UI/UI Code Generator")]
        static public void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<UICodeGeneratorWindow>();
            window.minSize = new Vector2(400, 450);
            window.maxSize = new Vector2(400, 450);
            window.titleContent.text = "UI代码生成器";
            window.Show();
        }

        void OnEnable()
        {
            InitData();
        }

        static void InitData()
        {
            param = new UICodeParam();
            param.types = new List<Type>()
            {
                typeof(Text),
                typeof(Button),
                typeof(Image),
                typeof(Dropdown),
                typeof(InputField),
                typeof(Transform),
            };
            param.typesName = new string[param.types.Count];
            param.uiRootPath = EditorPrefs.GetString(uiRootPath_tag, "Assets/Resource_local/UI");
            param.codeRootPath = EditorPrefs.GetString(codeRootPath_tag, "Assets/Scripts/GameModule");
            param.nameSpace = EditorPrefs.GetString(nameSpace_tag, "GameModule");
            param.templatePath = EditorPrefs.GetString(templatePath_tag, "Editor/UITool/UI模板.txt");
            param.separator = EditorPrefs.GetString(separator_tag, "_");

            var loadedData = JsonConvert.DeserializeObject<string[]>(EditorPrefs.GetString(types_tag));
            var length = Math.Min(loadedData.Length, param.typesName.Length);
            for (var i = 0; i < length; i++)
            {
                param.typesName[i] = loadedData[i];
            }
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(uiRootPath_tag, param.uiRootPath);
            EditorPrefs.SetString(codeRootPath_tag, param.codeRootPath);
            EditorPrefs.SetString(types_tag, JsonConvert.SerializeObject(param.typesName));
            EditorPrefs.SetString(separator_tag, param.separator);
            EditorPrefs.SetString(nameSpace_tag, param.nameSpace);
            EditorPrefs.SetString(templatePath_tag, param.templatePath);
        }

        public void OnGUI()
        {
            // 代码根目录
            param.codeRootPath = EditorGUILayout.TextField(codeRootPath_tag, param.codeRootPath);
            // UI根目录
            param.uiRootPath = EditorGUILayout.TextField(uiRootPath_tag, param.uiRootPath);
            // 命名空间
            param.nameSpace = EditorGUILayout.TextField(nameSpace_tag, param.nameSpace);
            // 模板文件路径
            param.templatePath = EditorGUILayout.TextField(templatePath_tag, param.templatePath);
            // 前后缀分隔符
            param.separator = EditorGUILayout.TextField(separator_tag, param.separator);
            // 前后缀映射关系
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("映射关系");
            var length = param.types.Count;
            for (var i = 0; i < length; i++)
            {
                param.typesName[i] = EditorGUILayout.TextField(param.types[i].ToString(), param.typesName[i]);
            }

            // 代码生成
            if (GUILayout.Button("生成代码"))
            {
                GenerateCode();
            }
        }

        public static void GenerateCode()
        {
            UICodeGenerator.GetSelectedUI(out var goes, out var paths, out var guids);
            UICodeGenerator.TypeMap.Clear();
            var length = param.types.Count;
            for (var i = 0; i < length; i++)
            {
                UICodeGenerator.TypeMap.Add(param.typesName[i], param.types[i]);
            }
            UICodeGenerator.Separator = param.separator;
            // 读取模板
            param.template = Framework.Util.FileUtil.ReadFileToString(Application.dataPath + "/" + param.templatePath);

            for (var j = 0; j < goes.Count; j++)
            {
                var code = UICodeGenerator.GenerateUICode(goes[j], paths[j], guids[j], param);
                if (string.IsNullOrEmpty(code))
                {
                    Debug.LogError($"failed to generate {paths[j]} code");
                    return;
                }
                var relatedPath = paths[j].Replace(param.uiRootPath, string.Empty);
                var codePath = Application.dataPath + "/" + param.codeRootPath + "/" + relatedPath + ".cs";
                codePath = codePath.Remove(codePath.IndexOf("Assets/"), "Assets/".Length);
                Framework.Util.FileUtil.WriteFile(codePath, code);
                Debug.Log($"generate {paths[j]} code successfully");
            }
        }

        public static void CreateUICode()
        {
            InitData();
            GenerateCode();
        }
    }

    public class UICodeGenerator
    {
        public static Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();

        public static string Separator;

        public static void GetSelectedUI(out List<GameObject> result, out List<string> paths, out List<string> guids)
        {
            result = new List<GameObject>();
            paths = new List<string>();
            guids = new List<string>();
            var suffix = ".prefab";
            var goes = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            foreach (var go in goes)
            {
                if (!(go is GameObject)) continue;
                var path = AssetDatabase.GetAssetPath(go);
                if (!path.EndsWith(suffix)) continue;
                result.Add(go as GameObject);
                var guid = AssetDatabase.AssetPathToGUID(path);
                guids.Add(guid);
                path = path.Replace(suffix, string.Empty);
                paths.Add(path);

            }
        }

        /// <summary>
        /// 生成UI代码
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="uiPath"></param>
        /// <param name="uiRootPath"></param>
        /// <param name="codeRootPath"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static string GenerateUICode(GameObject prefab, string uiPath, string guid, UICodeParam param)
        {
            if (!uiPath.Contains(param.uiRootPath))
            {
                Debug.LogError($"UI {uiPath} 不在根目录{param.uiRootPath}下");
                return string.Empty;
            }

            if (!Directory.Exists(param.codeRootPath))
            {
                Debug.LogError($"代码根目录{param.codeRootPath}不存在");
                return string.Empty;
            }

            var address = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid).address;
            StringBuilder members = new StringBuilder();
            StringBuilder ctors = new StringBuilder();

            TraversePrefabNode(prefab.transform, string.Empty, (com, relatedPath) =>
            {
                Encode(com, relatedPath, out string memberCode, out string ctorCode);
                if (!string.IsNullOrEmpty(memberCode))
                {
                    members.AppendLine(memberCode);
                }
                if (!string.IsNullOrEmpty(ctorCode))
                {
                    ctors.AppendLine(ctorCode);
                }
            });
            var template = param.template;
            var code = template.Replace("NameSpace", param.nameSpace);
            code = code.Replace("ClassName", prefab.name);
            code = code.Replace("address", address);
            code = code.Replace("Members", members.ToString());
            code = code.Replace("ctor", ctors.ToString().TrimEnd('\r', '\n'));
            return code;
        }

        public static void Encode(Component com, string relatedPath, out string memberCode, out string ctorCode)
        {
            // 检测是否记录
            var name = com.name;
            memberCode = string.Empty;
            ctorCode = string.Empty;
            var parts = name.Split(new[] { Separator }, StringSplitOptions.None);
            if (parts.Length < 2) return;
            var flag = TypeMap.TryGetValue(parts[0], out var type);
            if (!flag) return;

            memberCode = $"\t\tpublic {type} {parts[1]};";
            ctorCode = $"\t\t\t\tRoot.Find(\"{relatedPath}\").TryGetComponent(out {parts[1]});";
        }

        public static void TraversePrefabNode(Component prefab, string relatedPath, Action<Component, string> cb)
        {
            cb?.Invoke(prefab, relatedPath);
            var childCount = prefab.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = prefab.transform.GetChild(i);
                TraversePrefabNode(child, relatedPath + child.name + "/", cb);
            }
        }
    }
}

public struct UICodeParam
{
    public string uiRootPath;
    public string codeRootPath;
    public string[] typesName;
    public List<Type> types;
    public string separator;
    public string nameSpace;
    public string templatePath;
    public string template;
}
