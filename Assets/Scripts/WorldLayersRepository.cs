using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class WorldLayersRepository
{
    private static HashSet<WorldLayer> _layers;

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        _layers = new HashSet<WorldLayer>();
    }

    public static void TryAddLayer(WorldLayer worldLayer)
    {
        _layers.Add(worldLayer);
    }
}
