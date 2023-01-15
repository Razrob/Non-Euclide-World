using System;
using UnityEngine;

[Serializable] 
public struct WorldLayerID
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

    public static implicit operator WorldLayerID(int id) => new WorldLayerID(id);
    public static implicit operator WorldLayerID(int? id) => id ?? null;

    public static bool operator ==(WorldLayerID left, WorldLayerID right) => left._layerID == right._layerID;
    public static bool operator ==(WorldLayerID left, int right) => left._layerID == right;
    public static bool operator !=(WorldLayerID left, WorldLayerID right) => !(left == right);
    public static bool operator !=(WorldLayerID left, int right) => !(left == right);

    public override bool Equals(object obj) => obj is WorldLayerID other && Equals(other);
    public override int GetHashCode() => _layerID;
    public override string ToString() => $"WorldLayerID: {_layerID}";
}