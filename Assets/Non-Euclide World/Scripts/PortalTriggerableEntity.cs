using System;
using UnityEngine;

public class PortalTriggerableEntity : MonoBehaviour, ITriggerable
{
    public Transform Transform => transform;

    public static PortalTriggerableEntity Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            throw new Exception("PortalTriggerableEntity should be only one");

        Instance = this;
    }
}