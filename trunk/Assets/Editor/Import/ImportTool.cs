using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using FileUtil = Framework.Util.FileUtil;

public class ImportTool : MonoBehaviour
{
    public static void SetAssetAddress(string assetPath, string address, string groupName = "", string lable = "")
    {
        var setting = AddressableAssetSettingsDefaultObject.Settings;
        if (string.IsNullOrEmpty(groupName))
            groupName = FileUtil.GetParentDirName(assetPath);

        var group = setting.FindGroup(groupName);
        if (group == null)
            group = setting.CreateGroup(groupName, false, false, true,
                    new List<AddressableAssetGroupSchema> { setting.DefaultGroup.Schemas[0], setting.DefaultGroup.Schemas[1] }, typeof(SchemaType));

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogError($"asset:{assetPath} is not exist!");
            return;
        }

        var entry = setting.CreateOrMoveEntry(guid, group);
        if (entry == null)
        {
            Debug.LogError($"create entry fault");
            return;
        }


        entry.address = address;
        if (string.IsNullOrEmpty(lable))
            entry.SetLabel("default", true, true);
        Debug.Log($"asset:{assetPath} is added to group:{groupName}");
    }

    public static void RemoveAssetAddress(string assetPath)
    {
        var setting = AddressableAssetSettingsDefaultObject.Settings;
        if (assetPath.Contains(Application.dataPath))
        {
            assetPath = assetPath.Replace(Application.dataPath, "");
            assetPath = "Assets" + assetPath;
        }
        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogError($"asset:{assetPath} is not exist!");
            return;
        }
        setting.RemoveAssetEntry(guid);
        Debug.Log($"asset:{assetPath} is removed from Addressable System");
    }

    public static void RemoveAddressableGroup(string groupName) 
    {
        var setting = AddressableAssetSettingsDefaultObject.Settings;
        var group = setting.FindGroup(groupName);
        setting.RemoveGroup(group);
    }
}
