using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldLayersRepository))]
public class WorldLayerRepositoryWindow : EditorWindow
{
    private WorldLayerRepositoryWindow _window;

    [MenuItem("NonEuclide World/Create Window")]
    private static void CreateWindow()
    {
        WorldLayerRepositoryWindow window = CreateWindow<WorldLayerRepositoryWindow>();
    }

    private void OnGUI()
    {
        
    }
}