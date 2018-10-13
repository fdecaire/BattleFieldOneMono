using System;

namespace BattlefieldOneMono
{
	public class BattleFieldOneCommonObjects
	{
		public static double ComputeCenterX(int x)
		{
			return 30 + (60 * x);
		}

		public static double ComputeCenterY(int x, int y)
		{
			return 35 + (35 * (x % 2) + y * 70);
		}

		public static double Distance(int sx, int sy, int ex, int ey)
		{
			// convert the map index coordinates into screen coordinates to get the correct distance
			var lnSx = ComputeCenterX(sx);
			var lnSy = ComputeCenterY(sx, sy);
			var lnEx = ComputeCenterX(ex);
			var lnEy = ComputeCenterY(ex, ey);

			return Math.Sqrt(Math.Pow(lnSx - lnEx, 2) + Math.Pow(lnSy - lnEy, 2));
		}
	}
}
