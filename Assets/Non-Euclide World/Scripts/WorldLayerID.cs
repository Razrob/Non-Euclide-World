using System;
using UnityEngine;

[Serializable] 
public class WorldLayerID
{
    [SerializeField] private int _layerID;
    public int LayerID => _layerID;

#if UNITY_EDITOR

    public static string LayerIDFieldName => nameof(_layerID);

#endif

    public WorldLayerID(int layerID)
    {
        _layerID = layerID;
    }
}