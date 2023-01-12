using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldCore
{
    public const string PARENT_FOLDER_NAME = "Non-Euclide World";

    public const string WORLD_CONFIG_PATH = "Assets/Non-Euclide World/Resources/Configuration";
    public const string WORLD_CONFIG_PATH_RESOURCES_EXCLUDE = "Configuration";
    public const string WORLD_CONFIG_NAME = "WorldConfig";

    public const string INSTANCED_MATERIALS_LOCAL_FOLDER_PATH = "Materials/Instanced";

    public const string STENCIL_VALUE_SHADER_PARAMETER = "_StencilValue";

    public int? ActiveWorldLayerID { get; private set; }
    public int? NextWorldLayerID { get; private set; }
    public int? LastActiveWorldLayerID { get; private set; }

    private HashSet<Material> _instancedMaterials;

    private static WorldCore _instance;
    public static WorldCore Instance => TryInitialize();

    private static WorldCore TryInitialize()
    {
        if (_instance is null)
        {
            _instance = new WorldCore();

            WorldConfig.Instance.TrySubscribeOnValidateEvent(_instance.OnConfigValidate);
            InitWorld();
        }

        return _instance;
    }

//#if UNITY_EDITOR
//    [InitializeOnLoadMethod]
//#endif
    [RuntimeInitializeOnLoadMethod]
    public static void InitWorld()
    {
        Instance.OnConfigValidate();
    }

    private void OnConfigValidate()
    {
        if (WorldLayersRepository.Instance.RegisteredLayers.Count is 0)
            return;

        CheckMeshRenderers();
        SetActiveLayer(WorldConfig.Instance.MainWorldLayerID);
    }

#if UNITY_EDITOR
    private string CalculateParentFolderPath()
    {
        string path = AssetDatabase.GetAssetPath(WorldConfig.Instance);

        while (path.Contains("/"))
        {
            int lastSlashIndex = path.LastIndexOf('/');
            string currentFolderName = path.CutFromFirst(path.LastIndexOf('/'));

            if (PARENT_FOLDER_NAME == currentFolderName)
                return path;

            path = path.Substring(0, lastSlashIndex);
        }

        return "";
    }
#endif

    public void CheckMeshRenderers()
    {
#if UNITY_EDITOR

        if (WorldLayersRepository.Instance.RegisteredLayers.Count is 0)
            return;

        //try
        //{
        string instancedMaterialsFolder = $"{CalculateParentFolderPath()}/{INSTANCED_MATERIALS_LOCAL_FOLDER_PATH}";

        string[] mats = Directory.GetFiles(instancedMaterialsFolder);
        _instancedMaterials = new HashSet<Material>(mats.Select(m => AssetDatabase.LoadAssetAtPath<Material>(m)));

        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
        {
            foreach (MeshRenderer meshRenderer in worldLayer.MeshRenderers)
            {
                Material[] sharedMaterials = new Material[meshRenderer.sharedMaterials.Length];
                bool arrayWasChanged = false;

                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    Material meshMaterial = meshRenderer.sharedMaterials[i];

                    if (_instancedMaterials.Contains(meshMaterial))
                    {
                        int materialLayerID =
                            int.Parse(meshMaterial.name.Cut(meshMaterial.name.IndexOf('['), meshMaterial.name.IndexOf(']')));

                        if (materialLayerID == worldLayer.LayerID)
                            continue;

                        if (!WorldLayersRepository.Instance.RegisteredLayers.Exist(l => l.LayerID == materialLayerID))
                        {
                            meshMaterial.name = $"{meshMaterial.name.Replace($"[{materialLayerID}]", $"[{worldLayer.LayerID}]")}";
                            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(meshMaterial), meshMaterial.name);
                        }
                        else
                        {
                            sharedMaterials[i] = CreateMaterial(meshMaterial, worldLayer, instancedMaterialsFolder);
                            arrayWasChanged = true;
                        }
                    }
                    else
                    {
                        sharedMaterials[i] = CreateMaterial(meshMaterial, worldLayer, instancedMaterialsFolder);
                        arrayWasChanged = true;
                    }

                }


                if (arrayWasChanged)
                {
                    EditorUtility.SetDirty(meshRenderer);
                    meshRenderer.sharedMaterials = sharedMaterials;
                    EditorUtility.ClearDirty(meshRenderer);
                }
            }
        }

        AssetDatabase.SaveAssets();
        //}
        //catch (Exception exception)
        //{
        //    Debug.LogError($"During materials checking was thrown an exception: {exception.Message}\n{exception.StackTrace}");
        //}
#endif
    }

#if UNITY_EDITOR
    private Material CreateMaterial(Material origin, WorldLayer worldLayer, string instancedMaterialsFolder)
    {
        string firstNamePart = origin.name.Replace("(Clone)", "");
        string instancedMaterialName = $"{firstNamePart.CutFromSecond(firstNamePart.IndexOf("_["))}_[{worldLayer.LayerID}]_Instanced";

        string finalPath = $"{instancedMaterialsFolder}/{instancedMaterialName}.mat";

        Material instancedMaterial = AssetDatabase.LoadAssetAtPath<Material>(finalPath);

        if (!instancedMaterial)
        {
            instancedMaterial = new Material(origin);
            instancedMaterial.name = instancedMaterialName;
            AssetDatabase.CreateAsset(instancedMaterial, finalPath);
        }

        return instancedMaterial;
    }
#endif

    public void SetActiveLayer(int layerID, int? nextLayerID = null)
    {
        if (ActiveWorldLayerID.HasValue && ActiveWorldLayerID.Value == layerID
            && NextWorldLayerID.HasValue && (!nextLayerID.HasValue || NextWorldLayerID.Value == nextLayerID))
            return;

        LastActiveWorldLayerID = ActiveWorldLayerID;
        ActiveWorldLayerID = layerID;
        NextWorldLayerID = nextLayerID;

        foreach (WorldLayer worldLayer in WorldLayersRepository.Instance.RegisteredLayers)
        {
            bool active = worldLayer.LayerID == layerID;

            worldLayer.SetLayerActivity(active);
            worldLayer.SetLayerStencilParameter(STENCIL_VALUE_SHADER_PARAMETER, active ? 0 : worldLayer.LayerID);
        }
    }

    public void SetNextLayer(int nextLayerID)
    {
        SetActiveLayer(ActiveWorldLayerID.Value, nextLayerID);
    }
}