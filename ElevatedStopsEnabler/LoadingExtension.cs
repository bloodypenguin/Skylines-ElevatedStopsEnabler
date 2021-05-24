using System;
using CitiesHarmony.API;
using ElevatedStopsEnabler.HarmonyPatches.NetSegmentPatches;
using ElevatedStopsEnabler.HarmonyPatches.TransportLineAIPatches;
using ICities;
using ElevatedStopsEnabler.Util;

namespace ElevatedStopsEnabler
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            try
            {
                ElevatedStops.AddElevatedStoptypes();
                ElevatedStops.AllowStreetLightsOnElevatedStops();
                GetClosestLanePositionPatch.Apply();
                AddLaneConnectionPatch.Apply();
                RemoveLaneConnectionPatch.Apply();
                Log.Info("Patches deployed");
            }
            catch (Exception e)
            {
                Log.Error($"Failed deploying Patches: {e}");
            }
        }

        public override void OnLevelUnloading()
        {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            try
            {
                GetClosestLanePositionPatch.Undo();
                AddLaneConnectionPatch.Undo();
                RemoveLaneConnectionPatch.Undo();
                Log.Info("patching reverted");
            }
            catch (Exception e)
            {
                Log.Error($"Failed reverting patches: {e}");
            }
            base.OnLevelUnloading();
            Log.Info("level unloaded");
        }
    }
}