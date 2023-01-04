using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-15)]
public class WorldConfig : ScriptableObject
{
    [SerializeField] private WorldLayerID _mainWorldLayerID;

    public int MainWorldLayerID => _mainWorldLayerID.LayerID;

    public static WorldConfig _instance { get; private set; }
    public static WorldConfig Instance
    {
        get
        {
            if (_instance is null)
                Init();

            return _instance;
        }
    }

    public event Action OnValidateEvent;

    private void OnValidate()
    {
        OnValidateEvent?.Invoke();
    }

    private void Awake()
    {
        Init();
    }


#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        string configPath = $"{WorldCore.WORLD_CONFIG_PATH_RESOURCES_EXCLUDE}/{WorldCore.WORLD_CONFIG_NAME}";
        _instance = Resources.Load(configPath, typeof(WorldConfig)) as WorldConfig;
        _instance.OnValidateEvent = null;
    }
}
