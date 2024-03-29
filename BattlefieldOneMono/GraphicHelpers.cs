﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	public static class GraphicHelpers
	{
		public static Texture2D Pixel { get; set; }

		public static void DrawLine(Vector2 start, Vector2 end, Color color, int width = 1)
		{
			Vector2 edge = end - start;
			// calculate angle to rotate line
			float angle = (float)Math.Atan2(edge.Y, edge.X);

			GameContent.Sb.Draw(Pixel,
				new Rectangle(// rectangle defines shape of line and position of start of line
					(int)start.X,
					(int)start.Y,
					(int)edge.Length(), //sb will strech the texture to fill this rectangle
					width), //width of line, change this to make thicker line
				null,
				color, //color of line
				angle,     //angle of line (calulated above)
				new Vector2(0, 0), // point in line about which to rotate
				SpriteEffects.None,
				0);
		}
	}
}
