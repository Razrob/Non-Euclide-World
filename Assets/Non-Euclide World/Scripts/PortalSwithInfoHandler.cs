using System;
using System.Collections.Generic;

public static class PortalSwithInfoHandler
{
    private static readonly IReadOnlyDictionary<WorldPortalSwitchingMethod, Type> _infoTypes =
        new Dictionary<WorldPortalSwitchingMethod, Type>()
        {
            { WorldPortalSwitchingMethod.Switch_To_Next_Layer_With_Reverse, null },
            { WorldPortalSwitchingMethod.Switch_To_Previous_Layer_With_Reverse, null },
            { WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse, typeof(WorldPortal.SpecificSwitchInfo) },
            { WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse_Two_Sided, typeof(WorldPortal.SpecificSwitchInfo) },
        };

    public static bool TypesEquals(WorldPortalSwitchingMethod switchingMethod, WorldPortal.SwitchInfoBase existInfo)
    {
        return existInfo?.GetType() == _infoTypes[switchingMethod];
    }

    public static WorldPortal.SwitchInfoBase TryCreateInstance(WorldPortalSwitchingMethod switchingMethod, 
        WorldPortal.SwitchInfoBase existInfo)
    {
        if (TypesEquals(switchingMethod, existInfo))
            return existInfo;

        Type type = _infoTypes[switchingMethod];
        return type is null ? null : Activator.CreateInstance(type).Cast<WorldPortal.SwitchInfoBase>();
    }
}