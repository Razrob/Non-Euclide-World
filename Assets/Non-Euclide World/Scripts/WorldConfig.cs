using UnityEngine;
using System;
using System.Collections.Generic;

public class WorldConfig : ScriptableObject
{
    [SerializeField] private WorldLayerID _mainWorldLayerID;

    public int MainWorldLayerID => _mainWorldLayerID.LayerID;

    private static WorldConfig _instance;
    public static WorldConfig Instance => TryInitialize();

    private HashSet<Action> _onValidateListeners = new HashSet<Action>();

    private void OnValidate()
    {
        foreach (Action action in _onValidateListeners)
            action?.Invoke();
    }

    private static WorldConfig TryInitialize()
    {
        if (_instance is null)
        {
            string configPath = $"{WorldCore.WORLD_CONFIG_PATH_RESOURCES_EXCLUDE}/{WorldCore.WORLD_CONFIG_NAME}";
            _instance = Resources.Load(configPath, typeof(WorldConfig)) as WorldConfig;
            _instance.OnValidate();
        }

        return _instance;
    }

    public void TrySubscribeOnValidateEvent(Action action)
    {
        _onValidateListeners.Add(action);
    }
}
