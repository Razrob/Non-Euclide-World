#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(10)]
[CustomEditor(typeof(WorldConfig))]
public class WorldConfigEditor : Editor
{
    private WorldConfig _config;
    private string[] _worldLayerNames;

    private void OnEnable()
    {
        _config = (WorldConfig)target;
        _worldLayerNames = WorldLayersRepository.RegisteredLayers
            .OrderBy(layer => layer.LayerID)
            .Select(layer => TransformWorldLayerToLabel(layer))
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawMainLayerField();

        serializedObject.ApplyModifiedProperties();
    }

    private string TransformWorldLayerToLabel(WorldLayer worldLayer)
    {
        return $"[{worldLayer.name}] [{worldLayer.LayerID}]";
    }

    private void DrawMainLayerField()
    {
        SerializedProperty layerIDProperty = serializedObject.FindProperty(_config.MainWorldLayerIndexFieldName);

        if (WorldLayersRepository.RegisteredLayers.Count > 0)
        {
            WorldLayer mainWorldLayer = WorldLayersRepository.RegisteredLayers.Find(layer => layer.LayerID == layerIDProperty.intValue);

            if (mainWorldLayer is null)
            {
                mainWorldLayer = WorldLayersRepository.RegisteredLayers
                    .Find(layer => TransformWorldLayerToLabel(layer) == _worldLayerNames.First());

                layerIDProperty.intValue = mainWorldLayer.LayerID;
            }

            int selectedLayerNameIndex = _worldLayerNames.IndexOf(name => name == TransformWorldLayerToLabel(mainWorldLayer));

            if (selectedLayerNameIndex < 0)
                selectedLayerNameIndex = 0;

            int newSelectedNameIndex = EditorGUILayout.Popup(new GUIContent("MainLayer"), selectedLayerNameIndex, _worldLayerNames);
            int newSelectedWorldLayerID = WorldLayersRepository.RegisteredLayers
                .Find(layer => TransformWorldLayerToLabel(layer) == _worldLayerNames[newSelectedNameIndex]).LayerID;

            if (layerIDProperty.intValue != newSelectedWorldLayerID)
                layerIDProperty.intValue = newSelectedWorldLayerID;
        }
        else
        {
            GUILayout.Label(new GUIContent("World layers not found"), WorldEditorStyles.HelpBoxStyle);
        }
    }
}

#endif