using UnityEngine;

public class WorldLayer : MonoBehaviour
{
    [SerializeField] private int _layerIndex;

    [SerializeField, HideInInspector] private Collider[] _colliders;

    public int LayerIndex => _layerIndex;

#if UNITY_EDITOR

    public string CollidersFieldName => nameof(_colliders);

#endif

    private void OnValidate()
    {
        _colliders = GetComponentsInChildren<Collider>(true);
        WorldLayersRepository.TryAddLayer(this);
    }
}
