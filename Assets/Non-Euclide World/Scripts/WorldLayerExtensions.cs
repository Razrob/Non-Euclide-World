using System.Linq;

public static class WorldLayerExtensions
{
    public static WorldLayer CalculateNextLayer(int currentLayerID)
    {
        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
            if (worldLayer.LayerID > currentLayerID)
                return worldLayer;

        return WorldLayersRepository.Instance.RegisteredLayers.First();
    }

    public static WorldLayer CalculatePreviousLayer(int currentLayerID)
    {
        WorldLayer lastPreviousLayer = null;

        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
        {
            if (worldLayer.LayerID >= currentLayerID)
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