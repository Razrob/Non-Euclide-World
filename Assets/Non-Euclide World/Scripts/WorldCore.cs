using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class WorldCore
{
    public const string WORLD_CONFIG_PATH = "Assets/Non-Euclide World/Resources/Configuration";
    public const string WORLD_CONFIG_PATH_RESOURCES_EXCLUDE = "Configuration";
    public const string WORLD_CONFIG_NAME = "WorldConfig";

    public static int? ActiveWorldLayerID { get; private set; }
    public static int? LastActiveWorldLayerID { get; private set; }

    private static Shader _mainStandardShader;
    private static Shader _secondStandardShader;

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    private static void PreInit()
    {
        _mainStandardShader = Shader.Find("Stencil/StencilStandard_Default");
        _secondStandardShader = Shader.Find("Stencil/StencilStandard_Second");

        WorldConfig.Instance.OnValidateEvent += OnConfigValidate;

        if (Application.isPlaying)
            InitWorld();
    }

    private static void InitWorld()
    {
        OnConfigValidate();
    }

    private static void OnConfigValidate()
    {
        SetActiveLayers(WorldConfig.Instance.MainWorldLayerID);
    }

    public static void SetActiveLayers(int layerID, params int[] additional) 
    {
        //if (ActiveWorldLayerID.HasValue && ActiveWorldLayerID.Value == layerID)
        //    return;

        LastActiveWorldLayerID = ActiveWorldLayerID;
        ActiveWorldLayerID = layerID;

        foreach (WorldLayer worldLayer in WorldLayersRepository.RegisteredLayers)
        {
            bool active = worldLayer.LayerID == layerID || additional.Contains(id => id == worldLayer.LayerID);

            worldLayer.SetLayerActivity(active);
            worldLayer.SetLayerShader(active ? _mainStandardShader : _secondStandardShader);
        }
    }
}
