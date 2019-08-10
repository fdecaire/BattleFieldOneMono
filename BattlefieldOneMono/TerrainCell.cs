using System;
using BattlefieldOneMono.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	public class TerrainCell : ITerrainCell
	{
		private int X;
		private int Y;
		private int Z;
		public int BackgroundType { get; set; }
		public bool Mask { get; set; }
		public bool View { get; set; }
		public RoadType Roads { get; set; } // 0,1,2,4,8,16,32

		public double GroundUnitTerrainModifier
		{
			get
			{
				// roads or city give double speed for ground units
				if (Roads > 0 || IsCity)
				{
					return 0.5;
				}
				return 1;
			}
		}

		public bool IsCity => (BackgroundType == 40);

		/// <summary>
		/// use cube coordinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="backgroundType"></param>
		public TerrainCell(int x, int y, int z, int backgroundType)
		{
			X = x;
			Y = y;
			Z = z;
			BackgroundType = backgroundType;
			Mask = true;
			View = false;
			Roads = RoadType.None;
		}

		/// <summary>
		/// use hex coordinates
		/// </summary>
		/// <param name="q"></param>
		/// <param name="r"></param>
		/// <param name="backgroundType"></param>
		public TerrainCell(int q, int r, int backgroundType) : this(q, -q - r, r, backgroundType)
		{

		}

		/// <summary>
		/// return true if the unit in question is blocked by this terrain type
		/// </summary>
		/// <param name="unitType"></param>
		/// <returns></returns>
		public bool Blocked(int unitType)
		{
			if (BackgroundType >= 20 && BackgroundType <= 39) // add any blockable terrain number to this list
			{
				return true;
			}
			else if (BackgroundType >= 50 && BackgroundType <= 59 && (unitType == 1 || unitType == 2))
			{
				// tanks/artillery cannot penetrate forests
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Draw(bool alwaysView = false)
		{
			// X,Z are the center point of the hex cell
			int lnX = 60 * X + GameContent.XOffset - 40;
			int lnY = 35 * X + Z * 70 + GameContent.YOffset - 35;

			if (Mask && !alwaysView)
			{
				// masked area
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.Mask, new Vector2(lnX, lnY), Color.Black);
				GameContent.Sb.End();
			}
			else if (!View && !alwaysView)
			{
				// area where view is true should already be unmasked
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.TerrainSprite[BackgroundType], new Vector2(lnX, lnY), new Color(150, 150, 150, 255));
				GameContent.Sb.End();

				DrawRoads(lnX, lnY, true);
			}
			else
			{
				// visible area
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.TerrainSprite[BackgroundType], new Vector2(lnX, lnY), Color.White);
				GameContent.Sb.End();

				DrawRoads(lnX, lnY, false);
			}

			/*
			// test computer center used by A* algorithm
			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			Vector2 v = new Vector2((float)BattleFieldOneCommonObjects.ComputeCenterX(X), (float)BattleFieldOneCommonObjects.ComputeCenterY(X, Z));
			GameContent.Sb.DrawCircle(v, 2, 20, Color.Red);
			GameContent.Sb.End();
			*/

			//DrawSample();
		}

		private void DrawRoads(int x, int y, bool dim)
		{
			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

			// draw roads
			if ((Roads & RoadType.Road1) == RoadType.Road1)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[0], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			if ((Roads & RoadType.Road2) == RoadType.Road2)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[1], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			if ((Roads & RoadType.Road3) == RoadType.Road3)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[2], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			if ((Roads & RoadType.Road4) == RoadType.Road4)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[3], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			if ((Roads & RoadType.Road5) == RoadType.Road5)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[4], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			if ((Roads & RoadType.Road6) == RoadType.Road6)
			{
				GameContent.Sb.Draw(GameContent.RoadSprite[5], new Vector2(x, y), dim ? new Color(150, 150, 150, 255) : Color.White);
			}

			GameContent.Sb.End();
		}

		// this method is used for alignment purposes only
		public void DrawSample()
		{
			int lnX = 60 * X + GameContent.XOffset;
			int lnY = 35 * X + Z * 70 + GameContent.YOffset;
			var center = new Point(lnX, lnY);
			var offsets = new [] { new Point(40, 0), new Point(20, 35), new Point(-20, 35), new Point(-40, 0), new Point(-20, -35), new Point(20, -35) };

			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

			var start = new Vector2(center.X + 20, center.Y - 35);
			var end = new Vector2(0, 0);
			for (int i = 0; i < 6; i++)
			{
				end.X = center.X + offsets[i].X;
				end.Y = center.Y + offsets[i].Y;

				GraphicHelpers.DrawLine(start, end, Color.Black);

				start = end;
			}

			GameContent.Sb.End();
		}

		private Vector2 HexCorner(Vector2 center, int size, int i)
		{
			var angleDeg = 60.0 * i;

			var angleRad = Math.PI / 180.0 * angleDeg;

			return new Vector2(center.X + size * (float)Math.Cos(angleRad), center.Y + size * (float)Math.Sin(angleRad));
		}
	}
}
