using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public class Line
	{
		public Vector2 Start { get; set; }
		public Vector2 End { get; set; }

		public Line(int sx, int sy, int ex, int ey)
		{
			Start = new Vector2(sx,sy);
			End = new Vector2(ex,ey);
		}
	}
}
