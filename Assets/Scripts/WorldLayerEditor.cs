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
        SerializedProperty serializedProperty = serializedObject.FindProperty(_target.CollidersFieldName);

        GUIContent content = new GUIContent($"Colliders count: {serializedProperty.arraySize}");
        EditorGUILayout.HelpBox(content);
    }
}