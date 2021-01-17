using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEditor.AddressableAssets.Build;

/// <summary>
/// 层级面板拓展
/// </summary>
public class HierarchyExtension
{
    [MenuItem("GameObject/Copy Path", false, 0)]
    static void CopyNodePath()
    {
        CopyPath.CopyNodePath();
    }

    [MenuItem("GameObject/CustomUI/CustomButton", false, 1)]
    static void CreateCustomButton(MenuCommand menuCommand)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resource_local/UI/Components/Button.prefab");
        var go = Object.Instantiate(asset);
        go.name = "Button";
        PlaceUIElementRoot(go, menuCommand);
    }

    [MenuItem("GameObject/CustomUI/CustomImage", false, 2)]
    static void CreateCustomImage(MenuCommand menuCommand)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resource_local/UI/Components/Image.prefab");
        var go = Object.Instantiate(asset);
        go.name = "Image";
        PlaceUIElementRoot(go, menuCommand);
    }


    private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        bool explicitParentChoice = true;
        if (parent == null)
        {
            parent = GetOrCreateCanvasGameObject();
            explicitParentChoice = false;

            // If in Prefab Mode, Canvas has to be part of Prefab contents,
            // otherwise use Prefab root instead.
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                parent = prefabStage.prefabContentsRoot;
        }
        if (parent.GetComponentsInParent<Canvas>(true).Length == 0)
        {
            // Create canvas under context GameObject,
            // and make that be the parent which UI element is added under.
            GameObject canvas = CreateCanvas();
            Undo.SetTransformParent(canvas.transform, parent.transform, "");
            parent = canvas;
        }

        GameObjectUtility.EnsureUniqueNameForSibling(element);

        SetParentAndAlign(element, parent);
        if (!explicitParentChoice) // not a context click, so center in sceneview
            SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

        // This call ensure any change made to created Objects after they where registered will be part of the Undo.
        Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");

        // We have to fix up the undo name since the name of the object was only known after reparenting it.
        Undo.SetCurrentGroupName("Create " + element.name);

        Selection.activeGameObject = element;
    }

    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (IsValidCanvas(canvas))
            return canvas.gameObject;

        Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
        for (int i = 0; i < canvasArray.Length; i++)
            if (IsValidCanvas(canvasArray[i]))
                return canvasArray[i].gameObject;


        return CreateCanvas();
    }

    private static void SetParentAndAlign(GameObject child, GameObject parent)
    {
        if (parent == null)
            return;

        Undo.SetTransformParent(child.transform, parent.transform, "");

        RectTransform rectTransform = child.transform as RectTransform;
        if (rectTransform)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            Vector3 localPosition = rectTransform.localPosition;
            localPosition.z = 0;
            rectTransform.localPosition = localPosition;
        }
        else
        {
            child.transform.localPosition = Vector3.zero;
        }
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        SetLayerRecursively(child, parent.layer);
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i).gameObject, layer);
    }

    static public GameObject CreateCanvas()
    {

        var root = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.layer = LayerMask.NameToLayer("UI");
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Works for all stages        
        StageUtility.PlaceGameObjectInCurrentStage(root);
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
        }

        Undo.SetCurrentGroupName("Create " + root.name);

        return root;
    }

    static bool IsValidCanvas(Canvas canvas)
    {
        if (canvas == null || !canvas.gameObject.activeInHierarchy)
            return false;

        if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
            return false;

        if (StageUtility.GetStageHandle(canvas.gameObject) != StageUtility.GetCurrentStageHandle())
            return false;

        return true;
    }

    private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
    {
        SceneView sceneView = SceneView.lastActiveSceneView;

        // Couldn't find a SceneView. Don't set position.
        if (sceneView == null || sceneView.camera == null)
            return;

        // Create world space Plane from canvas position.
        Vector2 localPlanePosition;
        Camera camera = sceneView.camera;
        Vector3 position = Vector3.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
        {
            // Adjust for canvas pivot
            localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
            localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

            localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
            localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

            // Adjust for anchoring
            position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
            position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

            Vector3 minLocalPosition;
            minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
            minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

            Vector3 maxLocalPosition;
            maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
            maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

            position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
            position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
        }

        itemTransform.anchoredPosition = position;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localScale = Vector3.one;
    }
}
