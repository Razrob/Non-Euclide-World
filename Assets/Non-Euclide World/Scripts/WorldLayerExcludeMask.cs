using UnityEngine;

public class WorldLayerExcludeMask : MonoBehaviour 
{
    [SerializeField] private bool _excludeMeshRenderer;
    [SerializeField] private bool _excludeCollider;

    public bool ExcludeMeshRenderer => _excludeMeshRenderer;
    public bool ExcludeCollider => _excludeCollider;
}