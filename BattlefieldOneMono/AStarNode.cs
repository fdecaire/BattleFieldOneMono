using System;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public class AStarNode
	{
		public double F => G + H;
		public double G { get; set; }
		public double H { get; set; }
		public int X { get; }
		public int Y { get; }
		public Point Source;

		public AStarNode(int sourceX, int sourceY, int x, int y, int endX, int endY, double distance)
		{
			this.X = x;
			this.Y = y;

			Source.X = sourceX;
			Source.Y = sourceY;

			G = distance;
			H = Distance(x, y, endX, endY);
		}

		private double Distance(int x, int y, int endX, int endY)
		{
			return Math.Sqrt(Math.Pow(x - endX, 2) + Math.Pow(y - endY, 2));
		}
	}
}
