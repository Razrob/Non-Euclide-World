using UnityEngine;

[ExecuteAlways]
public class WorldLayer : MonoBehaviour
{
    [SerializeField] private int _layerID;

    [SerializeField, HideInInspector] private Collider[] _colliders;
    [SerializeField, HideInInspector] private MeshRenderer[] _meshRenderers;

    public int LayerID => _layerID;

#if UNITY_EDITOR

    public string CollidersFieldName => nameof(_colliders);
    public string MeshRenderersFieldName => nameof(_meshRenderers);

#endif 

    private void OnValidate()
    {
        _colliders = GetComponentsInChildren<Collider>(true);
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);

        WorldLayersRepository.TryRegisterLayer(this);
    }

    public void SetLayerShader(Shader shader)
    {
        foreach (MeshRenderer meshRenderer in _meshRenderers)
            foreach (Material material in meshRenderer.sharedMaterials)
                material.shader = shader;
    }

    public void SetLayerActivity(bool value)
    {
        foreach (Collider collider in _colliders)
            collider.enabled = value;
    }

    private void OnDestroy()
    {
        WorldLayersRepository.TryUnregisterLayer(this);
    }
}
