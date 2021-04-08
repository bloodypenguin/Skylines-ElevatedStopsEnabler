using ElevatedStopsEnabler.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatedStopsEnabler
{
    static class ElevatedStops
    {
        public static void AddElevatedStoptypes()
        {
            var networks = UnityEngine.Resources.FindObjectsOfTypeAll<NetInfo>();
            foreach (var network in networks)
            {
                if (!network.m_hasPedestrianLanes)
                {
                    continue;
                }
                RoadAI ai = network.m_netAI as RoadAI;
                if (ai == null)
                {
                    continue;
                }
                bool hasStops = network.m_lanes.Any(lane => lane.m_stopType != VehicleInfo.VehicleType.None);
                if (!hasStops)
                {
                    continue;
                }
                VehicleInfo.VehicleType firstStopType = network.m_lanes[network.m_sortedLanes[0]].m_stopType;
                VehicleInfo.VehicleType secondStopType = network.m_lanes[network.m_sortedLanes[network.m_sortedLanes.Length - 1]].m_stopType;
                VehicleInfo.VehicleType mediumStopType = VehicleInfo.VehicleType.None;
                for (int i = 1; i < network.m_sortedLanes.Length - 2; i++)
                {
                    if (network.m_lanes[network.m_sortedLanes[i]].m_stopType != VehicleInfo.VehicleType.None)
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

        private static void EnableStops(NetInfo info, VehicleInfo.VehicleType mediumStopType, VehicleInfo.VehicleType firstStopType, VehicleInfo.VehicleType secondStopType)
        {
            try
            {
                if (info == null) return;
                if (info.m_lanes.Length == 0 || info.m_sortedLanes.Length == 0)
                {
                    Log.Error($"[ElevatedStops] custom road {info} without lanes found. Can't enable Elevated Stops!");
                    return;
                }
                for (int i = 1; i < info.m_lanes.Length - 2; i++)
                {
                    if (info.m_lanes[info.m_sortedLanes[i]].m_vehicleType == VehicleInfo.VehicleType.None)
                    {
                        info.m_lanes[info.m_sortedLanes[i]].m_stopType = mediumStopType;
                    }
                }
                info.m_lanes[info.m_sortedLanes[0]].m_stopType = firstStopType;
                info.m_lanes[info.m_sortedLanes[0]].m_stopOffset = 0f;
                info.m_lanes[info.m_sortedLanes[info.m_sortedLanes.Length - 1]].m_stopType = secondStopType;
                info.m_lanes[info.m_sortedLanes[info.m_sortedLanes.Length - 1]].m_stopOffset = 0f;
            }
            catch (Exception e)
            {
                Log.Error($"Failed on EnableStops {e}");
            }
        }
    }

    public static class ElevatedStopsExtensions
    {
        public static bool IsValidTransport(this VehicleInfo.VehicleType vehicleType)
        {
            return vehicleType == VehicleInfo.VehicleType.Car
                || vehicleType == VehicleInfo.VehicleType.Tram
                || vehicleType == VehicleInfo.VehicleType.Trolleybus;
        }

        public static bool IsValidTransport(this TransportInfo.TransportType transportType)
        {
            return transportType == TransportInfo.TransportType.Trolleybus
                || transportType == TransportInfo.TransportType.Bus
                || transportType == TransportInfo.TransportType.TouristBus
                || transportType == TransportInfo.TransportType.Tram;
        }
    }
}
