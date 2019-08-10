using System;

namespace BattlefieldOneMono
{
	[Flags]
	public enum RoadType
	{
		None = 0,
		Road1 = 0x01,
		Road2 = 0x02,
		Road3 = 0x04,
		Road4 = 0x08,
		Road5 = 0x10,
		Road6 = 0x20
	}
}
