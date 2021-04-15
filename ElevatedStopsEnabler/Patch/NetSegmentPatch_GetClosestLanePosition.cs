using System;
using ColossalFramework;
using ElevatedStopsEnabler.Util;
using UnityEngine;

namespace ElevatedStopsEnabler.Patch
{
    class NetSegmentPatch_GetClosestLanePosition
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(NetSegment), nameof(NetSegment.GetClosestLanePosition),
                    argumentTypes:new[] { typeof(Vector3), typeof(NetInfo.LaneType), typeof(VehicleInfo.VehicleType), typeof(VehicleInfo.VehicleType),
                    typeof(bool),
                    typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType(), typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType()}),
                new PatchUtil.MethodDefinition(typeof(NetSegmentPatch_GetClosestLanePosition), nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(NetSegment), nameof(NetSegment.GetClosestLanePosition),
                    argumentTypes:new[] { typeof(Vector3), typeof(NetInfo.LaneType), typeof(VehicleInfo.VehicleType), typeof(VehicleInfo.VehicleType),
                        typeof(bool),
                        typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType(), typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType()})
            );
        }

        static bool Prefix(ref NetSegment __instance, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, ref bool requireConnect)
        {
            if (!requireConnect) return true;
            __instance.GetClosestLane(0, laneTypes, vehicleTypes, out int laneindex, out uint laneID);
            var segment = Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_segment;
            var roadAi = Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info.m_netAI;
            if (!(roadAi is RoadBridgeAI)) return true;
            requireConnect = false;
            return true;
        }
    }
}
