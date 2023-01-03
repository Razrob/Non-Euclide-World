using System.Collections.Generic;

public class WorldLayerComparer : IComparer<WorldLayer>
{
    public int Compare(WorldLayer x, WorldLayer y)
    {
        return Comparer<int>.Default.Compare(x.LayerID, y.LayerID);
    }
}