using System;
using System.Linq;
using ICities;
using ElevatedStopsEnabler.Util;
using CitiesHarmony.API;

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

                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Patcher.PatchAll();
                    Log.Info("Patches deployed");
                }
                else Log.Info("Harmony not found");
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
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Patcher.UnpatchAll();
                    Log.Info("patching reverted");
                }
                else Log.Info("Harmony not found");
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