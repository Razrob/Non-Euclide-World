using System;
using UnityEngine;

public class PortalTriggerableEntity : MonoBehaviour, ITriggerable
{
    [SerializeField] private Camera _camera;

    public Transform Transform => transform;
    public Camera Camera => _camera;
}