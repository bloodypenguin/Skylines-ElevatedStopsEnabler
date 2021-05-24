using ColossalFramework;
using ElevatedStopsEnabler.Util;

namespace ElevatedStopsEnabler.Patch
{

    internal class TransportLineAIPatch_RemoveLaneConnection
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportLineAI), "RemoveLaneConnection"),
                new PatchUtil.MethodDefinition(typeof(TransportLineAIPatch_RemoveLaneConnection), nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportLineAI), "RemoveLaneConnection")
            );
        }
        
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