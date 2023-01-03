using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class WorldLayersRepository
{
    private static SortedSet<WorldLayer> _layers;

    public static IReadOnlyCollection<WorldLayer> RegisteredLayers => _layers;

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        if (_layers is null) 
            _layers = new SortedSet<WorldLayer>(new WorldLayerComparer());
    }

    public static void TryRegisterLayer(WorldLayer worldLayer)
    {
        Initialize();

        _layers.Add(worldLayer);
    }

    public static void TryUnregisterLayer(WorldLayer worldLayer)
    {
        Initialize();

        _layers.Remove(worldLayer);
    }

    public static WorldLayer GetWithID(int id) => _layers.Find(l => l.LayerID == id);
}
