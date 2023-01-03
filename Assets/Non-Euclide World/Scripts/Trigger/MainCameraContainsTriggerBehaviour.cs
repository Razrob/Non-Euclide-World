using System;
using UnityEngine;

public class MainCameraContainsTriggerBehaviour : TriggerZone
{
    protected override Func<ITriggerable, bool> EnteredComponentIsSuitable => 
        triggerable => triggerable.Transform.TryGetComponent(out Camera camera);
}
