using ColossalFramework;
using ElevatedStopsEnabler.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ElevatedStopsEnabler.Patch
{
    [HarmonyPatch(typeof(TransportLineAI), "AddLaneConnection")]
    class TransportLineAIPatch_AddLaneConnection
    {
        static bool Prefix(NetLane.Flags ___m_stopFlag, VehicleInfo.VehicleType ___m_vehicleType, ushort nodeID, ref NetNode data, uint laneID, byte offset)
        {
            if (nodeID == 0 || !___m_vehicleType.IsValidTransport()) return true;
            ushort segment = Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_segment;
            Log.Debug($"AddLaneConn on segment {segment}");
            NetAI roadAi = Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info.m_netAI;
            if (!(roadAi is RoadBridgeAI)) return true;

            NetLane.Flags flags = (NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags;
            flags |= ___m_stopFlag;
            Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags = (ushort)flags;
            Log.Debug($"lane flags updated {(NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags}");
            if (roadAi is RoadBridgeAI roadBridgeAI)
                roadBridgeAI.UpdateSegmentStopFlags(segment, ref Singleton<NetManager>.instance.m_segments.m_buffer[segment]);
            Log.Debug($"segment flags updated {Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags}");
            return true;
        }
    }

    [HarmonyPatch(typeof(TransportLineAI), "RemoveLaneConnection")]
    class TransportLineAIPatch_RemoveLaneConnection
    {
        static bool Prefix(NetLane.Flags ___m_stopFlag, VehicleInfo.VehicleType ___m_vehicleType, ushort nodeID, ref NetNode data)
        {
            if (nodeID == 0 || !___m_vehicleType.IsValidTransport()) return true;
            ushort segment = Singleton<NetManager>.instance.m_lanes.m_buffer[data.m_lane].m_segment;
            Log.Debug($"RemoveLaneConn on segment {segment}");
            NetAI roadAi = Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info.m_netAI;
            if (!(roadAi is RoadBridgeAI)) return true;

            NetLane.Flags flags = (NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[data.m_lane].m_flags;
            flags &= ~___m_stopFlag;
            Singleton<NetManager>.instance.m_lanes.m_buffer[data.m_lane].m_flags = (ushort)flags;
            Log.Debug($"lane flags updated {(NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[data.m_lane].m_flags}");
            if (roadAi is RoadBridgeAI roadBridgeAI)
                roadBridgeAI.UpdateSegmentStopFlags(segment, ref Singleton<NetManager>.instance.m_segments.m_buffer[segment]);
            Log.Debug($"segment flags updated {Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags}");
            return true;
        }
    }
}
