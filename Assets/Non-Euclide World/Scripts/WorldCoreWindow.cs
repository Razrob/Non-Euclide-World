#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldCore))]
public class WorldCoreWindow : EditorWindow
{
    [MenuItem("NonEuclide World/Create Window")]
    private static void CreateWindow()
    {
        WorldCoreWindow window = CreateWindow<WorldCoreWindow>();
    }

    private void OnGUI()
    {
        GUILayout.Label("World Core");

        EditorGUILayout.BeginVertical(WorldEditorStyles.HelpBoxStyle);
        GUIStyle worldLayerLabelStyle = GUI.skin.label;
        worldLayerLabelStyle.fontSize = 15;

        foreach (WorldLayer worldLayer in WorldLayersRepository.RegisteredLayers)
        {
            EditorGUILayout.BeginHorizontal();

            string name = $"[{worldLayer.name}] -> LayerIndex: {worldLayer.LayerID}";
            GUILayout.Label(name, worldLayerLabelStyle);

            if (WorldConfig.Instance.MainWorldLayerID == worldLayer.LayerID)
            {
                GUIStyle style = WorldEditorStyles.MainLayerLabelStyle;
                style.fixedWidth = 50f;
                style.fixedHeight = worldLayerLabelStyle.fixedHeight;

                GUILayout.Label("[MAIN]", style);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }
}

#endif