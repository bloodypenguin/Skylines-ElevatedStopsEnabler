using System;
using ElevatedStopsEnabler.Patch;
using ICities;
using ElevatedStopsEnabler.Util;

namespace ElevatedStopsEnabler
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            Log.Info($"OnLevelLoaded: {mode}");
            base.OnLevelLoaded(mode);

            try
            {
                ElevatedStops.AddElevatedStoptypes();
                ElevatedStops.AllowStreetLightsOnElevatedStops();
                NetSegmentPatch_GetClosestLanePosition.Apply();
                TransportLineAIPatch_AddLaneConnection.Apply();
                TransportLineAIPatch_RemoveLaneConnection.Apply();
                Log.Info("Patches deployed");
            }
            catch (Exception e)
            {
                Log.Error($"Failed deploying Patches: {e}");
            }
        }

        public override void OnLevelUnloading()
        {
            try
            {
                NetSegmentPatch_GetClosestLanePosition.Undo();
                TransportLineAIPatch_AddLaneConnection.Undo();
                TransportLineAIPatch_RemoveLaneConnection.Undo();
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