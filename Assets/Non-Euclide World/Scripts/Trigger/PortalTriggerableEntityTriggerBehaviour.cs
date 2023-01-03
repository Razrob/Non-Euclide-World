
using System;

public class PortalTriggerableEntityTriggerBehaviour : TriggerZone 
{
    protected override Func<ITriggerable, bool> EnteredComponentIsSuitable 
        => triggerable => triggerable.Transform.TryGetComponent(out PortalTriggerableEntity portalTriggerableEntity);
}
