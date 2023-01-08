using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
[DefaultExecutionOrder(-500)] 
public class WorldLayer : MonoBehaviour
{
    [SerializeField] private int _layerID;

    [SerializeField, HideInInspector] private Collider[] _colliders;
    [SerializeField, HideInInspector] private MeshRenderer[] _meshRenderers;
    
    private ILayerChangeCallbackReceiver[] _callbackReceivers;

    public IReadOnlyList<MeshRenderer> MeshRenderers => _meshRenderers;
    public int LayerID => _layerID;

#if UNITY_EDITOR

    public string CollidersFieldName => nameof(_colliders);
    public string MeshRenderersFieldName => nameof(_meshRenderers);

#endif 

    private void OnValidate()
    {
        _colliders = GetComponentsInChildren<Collider>(true)
            .Where(m => !m.gameObject.TryGetComponent(out WorldLayerExcludeMask excludeMask) || !excludeMask.ExcludeCollider)
            .ToArray();

        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true)
            .Where(m => !m.gameObject.TryGetComponent(out WorldLayerExcludeMask excludeMask) || !excludeMask.ExcludeMeshRenderer)
            .ToArray();

        _callbackReceivers = GetComponentsInChildren<ILayerChangeCallbackReceiver>(true);

        WorldLayersRepository.Instance.TryRegisterLayer(this);
    }

    private void Awake()
    {
        OnValidate();
    }

    public void SetLayerStencilParameter(string parameterName, int value)
    {
        foreach (MeshRenderer meshRenderer in _meshRenderers)
            foreach (Material material in meshRenderer.sharedMaterials)
                material.SetInt(parameterName, value);
    }

    public void SetLayerActivity(bool value)
    {
        foreach (Collider collider in _colliders)
            collider.enabled = value;

        foreach (ILayerChangeCallbackReceiver receiver in _callbackReceivers)
        {
            if (value)
                receiver.OnLayerActivate();
            else
                receiver.OnLayerDeactivate();
        }
    }

    private void OnDestroy()
    {
        WorldLayersRepository.Instance.TryUnregisterLayer(this);
    }
}
