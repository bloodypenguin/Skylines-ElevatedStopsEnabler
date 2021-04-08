using CitiesHarmony.API;
using ElevatedStopsEnabler.Util;
using ICities;

namespace ElevatedStopsEnabler
{
    public class Mod : IUserMod
    {
        public string Name => "Elevated Stops Enabler";

        public string Description => "Allows to place transport stops on elevated versions of roads";

        public static string ModVersion => typeof(Mod).Assembly.GetName().Version.ToString();

        public void OnEnabled()
        {
            Log.Info($"ElevatedStops enabled {ModVersion}");
            HarmonyHelper.EnsureHarmonyInstalled();
        }
    }
}
