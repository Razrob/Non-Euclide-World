using System;

[AttributeUsage(AttributeTargets.Method)]
public class LayerCalculateSwitchMethodAttribute : Attribute
{
    public readonly WorldPortalSwitchingMethod SwitchingMethod;

    public LayerCalculateSwitchMethodAttribute(WorldPortalSwitchingMethod switchingMethod)
    {
        SwitchingMethod = switchingMethod;
    }

}