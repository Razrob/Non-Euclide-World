#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(10)]
[CustomEditor(typeof(WorldConfig))]
public class WorldConfigEditor : Editor
{
    private WorldConfig _config;

    private void OnEnable()
    {
        _config = (WorldConfig)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

#endif