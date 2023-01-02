#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldLayer))]
public class WorldLayerEditor : Editor
{
    private WorldLayer _target;

    private void OnEnable()
    {
        _target = (WorldLayer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SerializedProperty collidersProperty = serializedObject.FindProperty(_target.CollidersFieldName);
        SerializedProperty meshRenderersProperty = serializedObject.FindProperty(_target.MeshRenderersFieldName);

        GUIContent collidersContent = new GUIContent($"Colliders count: {collidersProperty.arraySize}");
        GUIContent meshRenderersContent = new GUIContent($"Mesh renderers count: {meshRenderersProperty.arraySize}");

        GUIStyle gUIStyle = new GUIStyle("HelpBox");
        gUIStyle.fixedHeight = 0;

        EditorGUILayout.BeginVertical(gUIStyle);
        GUILayout.Label(collidersContent);
        GUILayout.Label(meshRenderersContent);
        EditorGUILayout.EndVertical();

        GUIStyle activeLabelStyle = WorldEditorStyles.MainLayerLabelStyle;
        activeLabelStyle.fontSize = 14;

        GUILayout.Label(new GUIContent(WorldConfig.Instance.MainWorldLayerID == _target.LayerID ? "[ENABLED]" : "[DISABLED]"),
            activeLabelStyle);
    }
}

#endif