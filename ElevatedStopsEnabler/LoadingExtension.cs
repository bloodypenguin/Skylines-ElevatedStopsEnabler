using System.Linq;
using ICities;

namespace ElevatedStopsEnabler
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            var networks = UnityEngine.Resources.FindObjectsOfTypeAll<NetInfo>();
            foreach (var network in networks)
            {
                if (!network.m_hasPedestrianLanes)
                {
                    continue;
                }
                var ai = network.m_netAI as RoadAI;
                if (ai == null)
                {
                    continue;
                }
                var hasStops = network.m_lanes.Any(lane => lane.m_stopType != VehicleInfo.VehicleType.None);
                if (!hasStops)
                {
                    continue;
                }
                VehicleInfo.VehicleType firstStopType = network.m_lanes[network.m_sortedLanes[0]].m_stopType;
                VehicleInfo.VehicleType secondStopType = network.m_lanes[network.m_sortedLanes[network.m_sortedLanes.Length - 1]].m_stopType;
                VehicleInfo.VehicleType mediumStopType = VehicleInfo.VehicleType.None;
                for(int i = 1;i< network.m_sortedLanes.Length - 1; i++)
                {
                    if (network.m_lanes[network.m_sortedLanes[i]].m_stopType !=
                        VehicleInfo.VehicleType.None)
                    {
                        mediumStopType = network.m_lanes[network.m_sortedLanes[i]].m_stopType;
                        break;
                    }
                }
                EnableStops(ai.m_elevatedInfo, mediumStopType, firstStopType, secondStopType);
                EnableStops(ai.m_bridgeInfo, mediumStopType, firstStopType, secondStopType);
                EnableStops(ai.m_tunnelInfo, mediumStopType, firstStopType, secondStopType);
                EnableStops(ai.m_slopeInfo, mediumStopType, firstStopType, secondStopType);
            }

        }

        private static void EnableStops(NetInfo info, VehicleInfo.VehicleType mediumStopType, VehicleInfo.VehicleType firstStopType,
            VehicleInfo.VehicleType secondStopType)
        {
            if (info != null)
            {
                for (int i = 1; i < info.m_sortedLanes.Length - 1; i++)
                {
                    if (info.m_lanes[info.m_sortedLanes[i]].m_vehicleType == VehicleInfo.VehicleType.None)
                    {
                        info.m_lanes[info.m_sortedLanes[i]].m_stopType = mediumStopType;
                    }
                }
                info.m_lanes[info.m_sortedLanes[0]].m_stopType = firstStopType;
                info.m_lanes[info.m_sortedLanes[0]].m_stopOffset = 0f;
                info.m_lanes[info.m_sortedLanes[info.m_sortedLanes.Length - 1]].m_stopType = secondStopType;
                info.m_lanes[info.m_sortedLanes[info.m_sortedLanes.Length - 1]].m_stopOffset = 0.0f;
            }
        }
    }
}