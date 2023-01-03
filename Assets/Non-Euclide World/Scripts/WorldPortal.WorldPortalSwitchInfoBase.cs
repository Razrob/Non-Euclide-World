using System;
using UnityEngine;

public partial class WorldPortal
{
    [Serializable]
    public abstract class WorldPortalSwitchInfoBase { }

    [Serializable]
    public class WorldPortalSpecificSwitchInfo : WorldPortalSwitchInfoBase
    {
        [SerializeField] private WorldLayerID _nextWorldLayer;

        public int NextWorldLayerID => _nextWorldLayer.LayerID;
        public int PreviousWorldLayerID { get; private set; }

        public void SetPreviousWorldLayerID(int id)
        {
            PreviousWorldLayerID = id;
        }
    }
}