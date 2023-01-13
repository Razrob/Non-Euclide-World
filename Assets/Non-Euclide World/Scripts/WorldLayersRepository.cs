using System.Collections.Generic;
using UnityEngine;

public class WorldLayersRepository
{
    private static WorldLayersRepository _instance;
    public static WorldLayersRepository Instance => TryInitialize();

    private SortedSet<WorldLayer> _layers;
    public IReadOnlyCollection<WorldLayer> RegisteredLayers => _layers;

    public WorldLayersRepository()
    {
        _layers = new SortedSet<WorldLayer>(new WorldLayerComparer());
    }

    private static WorldLayersRepository TryInitialize()
    {
        return _instance ?? (_instance = new WorldLayersRepository());
    }

    public void TryRegisterLayer(WorldLayer worldLayer)
    {
        if (_layers.Contains(worldLayer))
            return;

        _layers.Add(worldLayer);
    }

    public void TryUnregisterLayer(WorldLayer worldLayer)
    {
        _layers.Remove(worldLayer);
    }

    public WorldLayer GetWithID(int id)
    {
        return _layers.Find(l => l.LayerID == id);
    }
}
