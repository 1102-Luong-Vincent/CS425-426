// 文件: Assets/Editor/AutoGenerateSpriteAtlas.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;

public class AutoGenerateSpriteAtlas : EditorWindow
{
    private string sourceFolder = "Assets/YourTilesFolder";  // 默认要打 atlas 的文件夹 (你改成你的路径)
    private string atlasSaveFolder = "Assets";                // atlas 保存路径 (相对于 Assets)
    private string atlasName = "Auto_Tilemap_Atlas";          // atlas 的名字 (不含后缀)

    [MenuItem("Tools/Auto Generate Sprite Atlas")]
    static void OpenWindow()
    {
        var window = GetWindow<AutoGenerateSpriteAtlas>();
        window.titleContent = new GUIContent("Auto Sprite Atlas");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Auto Generate SpriteAtlas", EditorStyles.boldLabel);
        sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);
        atlasSaveFolder = EditorGUILayout.TextField("Atlas Save Folder", atlasSaveFolder);
        atlasName = EditorGUILayout.TextField("Atlas Name", atlasName);

        if (GUILayout.Button("Generate Atlas"))
        {
            GenerateAtlas();
        }
    }

    void GenerateAtlas()
    {
        if (!AssetDatabase.IsValidFolder(sourceFolder))
        {
            Debug.LogError($"Source Folder \"{sourceFolder}\" is not valid.");
            return;
        }

        // 创建新的 SpriteAtlas
        SpriteAtlas atlas = new SpriteAtlas();

        // 设置 PackingSettings (padding, blockOffset 等)
        var packSettings = atlas.GetPackingSettings();
        packSettings.padding = 2;               // 边缘 padding: 2 像素 (你可以改成 1 或其他)
        packSettings.enableTightPacking = false;
        packSettings.enableRotation = false;
        atlas.SetPackingSettings(packSettings);

        // 设置 TextureSettings (filter mode, compression 等)
        var texSettings = atlas.GetTextureSettings();
        texSettings.filterMode = FilterMode.Point;
        texSettings.readable = false;
        atlas.SetTextureSettings(texSettings);

        // （可选）Platform-specific settings
        // 不设置压缩、最大尺寸等，让 atlas 使用默认配置
        // atlas.SetPlatformSettings(...);

        // 将整个 sourceFolder 加入 atlas 的 pack list
        atlas.Add(new[] { AssetDatabase.LoadAssetAtPath<Object>(sourceFolder) });

        // 保存 atlas
        string atlasPath = Path.Combine(atlasSaveFolder, atlasName + ".spriteatlas");
        AssetDatabase.CreateAsset(atlas, atlasPath);
        AssetDatabase.SaveAssets();

        // 强制重新打包 (可选)
        SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log($"SpriteAtlas generated: {atlasPath}\n — include sprites from folder: {sourceFolder}");
    }
}
