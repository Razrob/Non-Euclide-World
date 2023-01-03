#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WorldLayerID))]
public class WorldLayerIDDrawer : PropertyDrawer
{
    private bool _inited;
    private string[] _worldLayerNames;

    private void TryInit()
    {
        if (_inited)
            return;

        _inited = true;
        _worldLayerNames = WorldLayersRepository.RegisteredLayers
            .Select(layer => TransformWorldLayerToLabel(layer))
            .ToArray();
    }

    private string TransformWorldLayerToLabel(WorldLayer worldLayer)
    {
        return $"[{worldLayer.name}] [{worldLayer.LayerID}]";
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TryInit();
        DrawMainLayerField(position, property, label);
    }

    private void DrawMainLayerField(Rect position, SerializedProperty layerIDProperty, GUIContent label)
    {
        SerializedProperty intIDProperty = layerIDProperty.FindPropertyRelative(WorldLayerID.LayerIDFieldName);

        if (WorldLayersRepository.RegisteredLayers.Count > 0)
        {
            WorldLayer mainWorldLayer = WorldLayersRepository.RegisteredLayers.Find(layer => layer.LayerID == intIDProperty.intValue);

            if (mainWorldLayer is null)
            {
                mainWorldLayer = WorldLayersRepository.RegisteredLayers
                    .Find(layer => TransformWorldLayerToLabel(layer) == _worldLayerNames.First()); 

                intIDProperty.intValue = mainWorldLayer.LayerID;
            }

            int selectedLayerNameIndex = _worldLayerNames.IndexOf(name => name == TransformWorldLayerToLabel(mainWorldLayer));

            if (selectedLayerNameIndex < 0)
                selectedLayerNameIndex = 0;

            int newSelectedNameIndex = EditorGUI.Popup(position, "MainLayer", selectedLayerNameIndex, _worldLayerNames);
            int newSelectedWorldLayerID = WorldLayersRepository.RegisteredLayers
                .Find(layer => TransformWorldLayerToLabel(layer) == _worldLayerNames[newSelectedNameIndex]).LayerID;

            if (intIDProperty.intValue != newSelectedWorldLayerID)
                intIDProperty.intValue = newSelectedWorldLayerID;
        }
        else
        {
            GUI.Label(position, new GUIContent("World layers not found"), WorldEditorStyles.HelpBoxStyle);
        }
    }
}

#endif