#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class WorldEditorValidator : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        ValidateWorld();
    }

    [InitializeOnLoadMethod]
    private static void ValidateWorld()
    {
        ValidateConfiguration();
    }

    private static void ValidateConfiguration()
    {
        string configPath = $"{WorldCore.WORLD_CONFIG_PATH}/{WorldCore.WORLD_CONFIG_NAME}.asset";
        WorldConfig worldConfig = (WorldConfig)AssetDatabase.LoadAssetAtPath(configPath, typeof(WorldConfig));

        if (worldConfig is null)
        {
            worldConfig = ScriptableObject.CreateInstance<WorldConfig>();
            AssetDatabase.CreateAsset(worldConfig, configPath);
        }
    }
}

#endif