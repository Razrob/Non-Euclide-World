﻿using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-15)]
public class WorldConfig : ScriptableObject
{
    [SerializeField] private int _mainWorldLayerID;

    public int MainWorldLayerID => _mainWorldLayerID;

    public static WorldConfig Instance { get; private set; }

    public event Action OnValidateEvent;

#if UNITY_EDITOR

    public string MainWorldLayerIndexFieldName => nameof(_mainWorldLayerID);

#endif

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
        Instance = Resources.Load(configPath, typeof(WorldConfig)) as WorldConfig;
        Instance.OnValidateEvent = null;
    }
}