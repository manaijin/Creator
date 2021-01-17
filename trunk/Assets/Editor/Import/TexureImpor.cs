using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using FileUtil = Framework.Util.FileUtil;

public class TexureImpor : AssetPostprocessor
{
    public void OnPreprocessTexture()
    {
        Debug.LogError(assetPath);
        // UI图片：Sprite
        if (assetPath.Contains("Art/Image"))
            ImportSprite();

        // 更新图集
        if (assetPath.Contains("Art/Image/Icon"))
            CreateDirAtlas(Path.GetDirectoryName(assetPath));
    }

    public void ImportSprite()
    {
        var importer = assetImporter as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("importer is null");
            return;
        }

        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.textureType = TextureImporterType.Sprite;
        importer.alphaIsTransparency = false;
        var setting = new TextureImporterSettings();
        importer.ReadTextureSettings(setting);
        setting.spriteGenerateFallbackPhysicsShape = false;
        setting.alphaIsTransparency = false;

        SetTextureByName(Path.GetFileNameWithoutExtension(assetPath), setting);
        importer.SetTextureSettings(setting);

        var name = Path.GetFileNameWithoutExtension(assetPath);
        var param = name.Split('_');
        var key = string.Empty;
        if (param == null || param.Length <= 2)
        {
            key = assetPath;
        }
        else
        {
            key += param[0];
            key += param[1];
        }
        ImportTool.SetAssetAddress(assetPath, key);
    }

    public void SetTextureByName(string name, TextureImporterSettings setting)
    {
        var param = name.Split('_');
        foreach (var item in param)
        {
            if (string.IsNullOrEmpty(item))
                continue;
            SetTextureByParam(item, setting);
        }
    }

    public void SetTextureByParam(string param, TextureImporterSettings setting)
    {
        param = param.ToLower();
        switch (param)
        {
            case "default":
                setting.textureType = TextureImporterType.Default;
                break;
            case "sprite":
                setting.textureType = TextureImporterType.Sprite;
                break;
            case "normal":
                setting.textureType = TextureImporterType.NormalMap;
                break;
            case "alpha":
                setting.alphaIsTransparency = true;
                break;
            case "mipmap":
                setting.streamingMipmaps = true;
                setting.mipmapEnabled = true;
                break;
        }
    }

    public void CreateDirAtlas(string packDir)
    {
        var atlasDir = Path.GetDirectoryName(packDir);
        var atlasName = "atlas_" + Path.GetFileName(packDir).ToLower();
        var fullPath = atlasDir + '/' + atlasName + ".spriteatlas";
        SpriteAtlas atlas;
        if (!File.Exists(fullPath))
        {
            atlas = new SpriteAtlas();
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            };
            atlas.SetPackingSettings(packSetting);

            var textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);

            var platformSetting = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            };
            atlas.SetPlatformSettings(platformSetting);
            AssetDatabase.CreateAsset(atlas, fullPath);
        }
        else
        {
            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(fullPath);
        }

        if (atlas == null)
        {
            Debug.LogError($"{fullPath} is not exist");
            return;
        }

        var packes = atlas.GetPackables();
        atlas.Remove(packes);
        atlas.Add(new[] { AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(packDir) });
        ImportTool.SetAssetAddress(fullPath, atlasName);
        AssetDatabase.SaveAssets();

        // 取消图集图片的Addressable
        var assetes = FileUtil.GetDirAllAssetFullPath(packDir);
        foreach (var asset in assetes)
        {
            ImportTool.RemoveAssetAddress(asset);
        }
    }
}