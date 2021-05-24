using ElevatedStopsEnabler.Util;
using UnityEngine;

namespace ElevatedStopsEnabler.HarmonyPatches.NetSegmentPatches
{
    internal class GetClosestLanePositionPatch
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(NetSegment), nameof(NetSegment.GetClosestLanePosition),
                    argumentTypes:new[] { typeof(Vector3), typeof(NetInfo.LaneType), typeof(VehicleInfo.VehicleType), typeof(VehicleInfo.VehicleType),
                    typeof(bool),
                    typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType(), typeof(Vector3).MakeByRefType(), typeof(int).MakeByRefType(), typeof(float).MakeByRefType()}),
                new PatchUtil.MethodDefinition(typeof(GetClosestLanePositionPatch), nameof(Prefix))
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
            if (requireConnect && __instance.Info.m_netAI is RoadBridgeAI)
            {
                requireConnect = false;
            }

            return true;
        }
    }
}
