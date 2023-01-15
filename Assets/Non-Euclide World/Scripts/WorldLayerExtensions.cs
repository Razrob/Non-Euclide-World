using System.Linq;

public static class WorldLayerExtensions
{
    public static WorldLayer CalculateNextLayer(WorldLayerID currentLayerID)
    {
        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
            if (worldLayer.LayerID > currentLayerID.LayerID)
                return worldLayer;

        return WorldLayersRepository.Instance.RegisteredLayers.First();
    }

    public static WorldLayer CalculatePreviousLayer(WorldLayerID currentLayerID)
    {
        WorldLayer lastPreviousLayer = null;

        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
        {
            if (worldLayer.LayerID >= currentLayerID.LayerID)
            {
                if (lastPreviousLayer is null)
                    break;

                return lastPreviousLayer;
            }
            else
                lastPreviousLayer = worldLayer;
        }

        return WorldLayersRepository.Instance.RegisteredLayers.Last();
    }
}