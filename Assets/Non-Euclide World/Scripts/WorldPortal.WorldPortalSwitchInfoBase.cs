using System;
using UnityEngine;

//do not rename (serialize reference)
public partial class WorldPortal
{
    [Serializable]
    public abstract class SwitchInfoBase { }

    [Serializable]
    public class SpecificSwitchInfo : SwitchInfoBase
    {
        [SerializeField] private WorldLayerID _nextWorldLayer;

        public WorldLayerID NextWorldLayerID => _nextWorldLayer;
        public WorldLayerID? PreviousWorldLayerID { get; private set; }

        public void SetPreviousWorldLayerID(WorldLayerID? id)
        {
            PreviousWorldLayerID = id;
        }
    }
}