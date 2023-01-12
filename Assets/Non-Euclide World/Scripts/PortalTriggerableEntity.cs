using System;
using UnityEngine;

public class PortalTriggerableEntity : MonoBehaviour, ITriggerable
{
    [SerializeField] private Camera _camera;

    public Transform Transform => transform;
    public Camera Camera => _camera;

    private static PortalTriggerableEntity _instance;
    public static PortalTriggerableEntity Instance
    {
        get
        {
            if (_instance is null)
                _instance = FindObjectOfType<PortalTriggerableEntity>(true);

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
}