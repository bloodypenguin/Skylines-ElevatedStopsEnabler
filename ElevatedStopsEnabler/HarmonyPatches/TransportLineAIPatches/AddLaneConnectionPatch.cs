using ColossalFramework;
using ElevatedStopsEnabler.Util;

namespace ElevatedStopsEnabler.HarmonyPatches.TransportLineAIPatches
{
    internal class AddLaneConnectionPatch
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportLineAI), "AddLaneConnection"),
                new PatchUtil.MethodDefinition(typeof(AddLaneConnectionPatch), nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportLineAI), "AddLaneConnection")
            );
        }
        
        static bool Prefix(NetLane.Flags ___m_stopFlag, VehicleInfo.VehicleType ___m_vehicleType, ushort nodeID, ref NetNode data, uint laneID, byte offset)
        {
            if (nodeID == 0 || !___m_vehicleType.IsValidTransport())
            {
                return true;
            }
            var segment = Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_segment;
            Log.Debug($"AddLaneConn on segment {segment}");
            var roadAi = Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info.m_netAI;
            if (roadAi is not RoadBridgeAI)
            {
                return true;
            }

            var flags = (NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags;
            flags |= ___m_stopFlag;
            Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags = (ushort)flags;
            Log.Debug($"lane flags updated {(NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneID].m_flags}");
            if (roadAi is RoadBridgeAI roadBridgeAI)
            {
                roadBridgeAI.UpdateSegmentStopFlags(segment,
                    ref Singleton<NetManager>.instance.m_segments.m_buffer[segment]);
            }
            Log.Debug($"segment flags updated {Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags}");
            return true;
        }
    }
}
