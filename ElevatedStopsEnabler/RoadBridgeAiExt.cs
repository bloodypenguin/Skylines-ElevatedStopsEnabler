using ColossalFramework;
using ElevatedStopsEnabler.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatedStopsEnabler
{
    static class RoadBridgeAiExt
    {
		public static void UpdateSegmentStopFlags(this RoadBridgeAI roadbridge, ushort segmentID, ref NetSegment data)
		{
			Log.Debug($"updateflags on {segmentID}");
			roadbridge.UpdateSegmentFlags(segmentID, ref data);
			NetSegment.Flags flags = data.m_flags & ~(NetSegment.Flags.StopRight | NetSegment.Flags.StopLeft | NetSegment.Flags.StopRight2 | NetSegment.Flags.StopLeft2);
			if (roadbridge.m_info.m_lanes == null) return;
			NetManager instance = Singleton<NetManager>.instance;
			bool inverted = (data.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None;
			uint lane = instance.m_segments.m_buffer[(int)segmentID].m_lanes;
			int i = 0;
			while (i < roadbridge.m_info.m_lanes.Length && lane != 0U)
			{
				NetLane.Flags laneFlags = (NetLane.Flags)instance.m_lanes.m_buffer[(int)((UIntPtr)lane)].m_flags;
				if ((laneFlags & NetLane.Flags.Stop) != NetLane.Flags.None)
				{
					if (roadbridge.m_info.m_lanes[i].m_position < 0f != inverted)
					{
						flags |= NetSegment.Flags.StopLeft;
					}
					else
					{
						flags |= NetSegment.Flags.StopRight;
					}
				}
				else if ((laneFlags & NetLane.Flags.Stop2) != NetLane.Flags.None)
				{
					if (roadbridge.m_info.m_lanes[i].m_position < 0f != inverted)
					{
						flags |= NetSegment.Flags.StopLeft2;
					}
					else
					{
						flags |= NetSegment.Flags.StopRight2;
					}
				}
				lane = instance.m_lanes.m_buffer[(int)((UIntPtr)lane)].m_nextLane;
				i++;
			}
			data.m_flags = flags;
			Log.Debug($"flags {flags}");
		}
	}
}
