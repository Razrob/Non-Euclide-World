using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class WorldLayerCalculator
{
    private static HashSet<NextLayerCalculateReason> _writeReasons;
    private static Dictionary<WorldPortalSwitchingMethod, Func<LayerCalculationInfo, WorldLayerID>> _calculators;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        _writeReasons = new HashSet<NextLayerCalculateReason>(new NextLayerCalculateReason[]
        {
            NextLayerCalculateReason.Change_Main_Layer,
        });

        IEnumerable<MethodInfo> methods = typeof(WorldLayerCalculator).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(method => method.GetCustomAttribute<LayerCalculateSwitchMethodAttribute>() != null);

        _calculators = new Dictionary<WorldPortalSwitchingMethod, Func<LayerCalculationInfo, WorldLayerID>>(methods.Count());
        
        foreach (MethodInfo methodInfo in methods)
        {
            LayerCalculateSwitchMethodAttribute methodAttribute = methodInfo.GetCustomAttribute<LayerCalculateSwitchMethodAttribute>();

            Func<LayerCalculationInfo, WorldLayerID> calculator 
                = methodInfo.CreateDelegate(typeof(Func<LayerCalculationInfo, WorldLayerID>))
                .Cast<Func<LayerCalculationInfo, WorldLayerID>>();

            _calculators.Add(methodAttribute.SwitchingMethod, calculator);
        }
    }

    public static WorldLayerID CalculateNextPortalLayer(LayerCalculationInfo calculateInfo)
    {
        return _calculators[calculateInfo.WorldPortal.SwitchingMethod](calculateInfo);
    }

    [LayerCalculateSwitchMethod(WorldPortalSwitchingMethod.Switch_To_Next_Layer_With_Reverse)]
    private static WorldLayerID CalculateToNextWithReverse(LayerCalculationInfo calculateInfo)
    {
        return calculateInfo.PortalEnterSide is PortalEnterSide.Forward
                    ? WorldLayerExtensions.CalculateNextLayer(calculateInfo.MainLayer).LayerID
                    : WorldLayerExtensions.CalculatePreviousLayer(calculateInfo.MainLayer).LayerID;
    }

    [LayerCalculateSwitchMethod(WorldPortalSwitchingMethod.Switch_To_Previous_Layer_With_Reverse)]
    private static WorldLayerID CalculateToPreviousWithReverse(LayerCalculationInfo calculateInfo)
    {
        return calculateInfo.PortalEnterSide is PortalEnterSide.Forward
                    ? WorldLayerExtensions.CalculatePreviousLayer(calculateInfo.MainLayer).LayerID
                    : WorldLayerExtensions.CalculateNextLayer(calculateInfo.MainLayer).LayerID;
    }

    [LayerCalculateSwitchMethod(WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse)]
    private static WorldLayerID CalculateToSpecificWithReverse(LayerCalculationInfo calculateInfo)
    {
        WorldLayerID nextLayer;

        WorldPortal.SpecificSwitchInfo info = calculateInfo.WorldPortal.SwitchInfo.Cast<WorldPortal.SpecificSwitchInfo>();

        if (calculateInfo.PortalEnterSide is PortalEnterSide.Forward)
        {
            nextLayer = info.NextWorldLayerID;

            if (_writeReasons.Contains(calculateInfo.LayerCalculateReason))
                info.SetPreviousWorldLayerID(calculateInfo.MainLayer);
        }
        else
        {
            if (info.PreviousWorldLayerID.HasValue)
            {
                nextLayer = info.PreviousWorldLayerID.Value;

                if (_writeReasons.Contains(calculateInfo.LayerCalculateReason))
                    info.SetPreviousWorldLayerID(calculateInfo.MainLayer);
            }
            else
                nextLayer = calculateInfo.MainLayer;
        }

        return nextLayer;
    }

    [LayerCalculateSwitchMethod(WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse_Two_Sided)]
    private static WorldLayerID CalculateToSpecificWithReverseTwoSided(LayerCalculationInfo calculateInfo)
    {
        WorldLayerID nextLayer;

        WorldPortal.SpecificSwitchInfo info = calculateInfo.WorldPortal.SwitchInfo.Cast<WorldPortal.SpecificSwitchInfo>();

        if (calculateInfo.PortalEnterSide is PortalEnterSide.Forward || !info.PreviousWorldLayerID.HasValue)
        {
            nextLayer = info.NextWorldLayerID;

            if (_writeReasons.Contains(calculateInfo.LayerCalculateReason))
                info.SetPreviousWorldLayerID(calculateInfo.MainLayer);
        }
        else
        {
            //if (info.PreviousWorldLayerID.HasValue)
            //{
                nextLayer = info.PreviousWorldLayerID.Value;

                if (_writeReasons.Contains(calculateInfo.LayerCalculateReason))
                    info.SetPreviousWorldLayerID(calculateInfo.MainLayer);
            //}
            //else
            //{
            //    nextLayer = calculateInfo.MainLayer;
            //}
        }

        return nextLayer;
    }
}
