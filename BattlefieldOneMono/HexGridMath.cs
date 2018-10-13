using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public static class HexGridMath
	{
		public static Hex PixelToHex(double x, double y)
		{
			double size = 40;

			x -= GameContent.XOffset;
			y -= GameContent.YOffset;

			var q = x * 2.0 / 3.0 / size;
			var r = (-x / 3.0 + Math.Sqrt(3.0) / 3.0 * y) / size;

			return Round(q, r);
		}

		public static Hex CubeToHex(Cube h) // # axial
		{
			var q = h.X;
			var r = h.Z;

			return new Hex(q, r);
		}

		public static Hex CubeToHex(int x, int y, int z)
		{
			var q = x;
			var r = z;

			return new Hex(q, r);
		}

		public static Cube HexToCube(Hex h) // # axial
		{
			var x = h.Q;
			var z = h.R;
			var y = -x - z;

			return new Cube(x, y, z);
		}

		public static Cube HexToCube(int q, int r)
		{
			var x = q;
			var z = r;
			var y = -x - z;

			return new Cube(x, y, z);
		}

		public static Hex Round(double q, double r)
		{
			var x = q;
			var z = r;
			var y = -x - z;

			return CubeToHex(Round(x, y, z));
		}

		public static Offset CubeToOffset(int x, int y, int z)
		{
			var col = x;
			var row = z + (x - (x & 1)) / 2;
			return new Offset(col, row);
		}

		public static Cube OffsetToCube(int col, int row)
		{
			var x = col;
			var z = row - (col - (col & 1)) / 2;
			var y = -x - z;

			return new Cube(x, y, z);
		}

		public static Cube Round(double x, double y, double z)
		{
			var rx = (int)Math.Round(x);
			var ry = (int)Math.Round(y);
			var rz = (int)Math.Round(z);

			var xDiff = Math.Abs(rx - x);
			var yDiff = Math.Abs(ry - y);
			var zDiff = Math.Abs(rz - z);

			if (xDiff > yDiff && xDiff > zDiff)
			{
				rx = -ry - rz;
			}
			else if (yDiff > zDiff)
			{
				ry = -rx - rz;
			}
			else
			{
				rz = -rx - ry;
			}

			return new Cube(rx, ry, rz);
		}

	}

	public class Cube
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }

		public Cube(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;

			Debug.Assert(X + Y + Z == 0);
		}

		public Vector2 ToPixel
		{
			get
			{
				var lnX = 60 * X + GameContent.XOffset;
				var lnY = 35 * X + Z * 70 + GameContent.YOffset;

				return new Vector2(lnX, lnY);
			}
		}

		public static List<Cube> Directions = new List<Cube> { new Cube(1, 0, -1), new Cube(1, -1, 0), new Cube(0, -1, 1), new Cube(-1, 0, 1), new Cube(-1, 1, 0), new Cube(0, 1, -1) };

		public static Cube Direction(int direction)
		{
			return Directions[direction];
		}

		public static Cube Neighbor(Cube hex, int direction)
		{
			return Add(hex, Direction(direction));
		}

		public static Cube Add(Cube a, Cube b)
		{
			return new Cube(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Cube Subtract(Cube a, Cube b)
		{
			return new Cube(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static int Length(Cube hex)
		{
			return (Math.Abs(hex.X) + Math.Abs(hex.Y) + Math.Abs(hex.Z)) / 2;
		}

		public static int Distance(Cube a, Cube b)
		{
			return Length(Subtract(a, b));
		}

		public Offset CubeToOffset()
		{
			var col = X;
			var row = Z + (X - (X & 1)) / 2;
			return new Offset(col, row);
		}
	}

	public class Offset : IComparable<Offset>
	{
		public int Col { get; }
		public int Row { get; }

		public Offset(int col, int row)
		{
			Col = col;
			Row = row;
		}

		public Cube ToCube
		{
			get
			{
				var x = Col;
				var z = Row - (Col - (Col & 1)) / 2;
				var y = -x - z;

				return new Cube(x, y, z);
			}
		}

		public Vector2 ToPixel
		{
			get
			{
				var cube = ToCube;
				var lnX = 60 * cube.X + GameContent.XOffset;
				var lnY = 35 * cube.X + cube.Z * 70 + GameContent.YOffset;

				return new Vector2(lnX, lnY);
			}
		}

		public int CompareTo(Offset obj)
		{
			if (Row.CompareTo(obj.Row) != 0)
			{
				return Row.CompareTo(obj.Row);
			}
			if (Col.CompareTo(obj.Col) != 0)
			{
				return Col.CompareTo(obj.Col);
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			var offsetObj = obj as Offset;
			if (offsetObj == null)
			{
				return false;
			}
			return Col.Equals(offsetObj.Col) && Row.Equals(offsetObj.Row);
		}

		public override int GetHashCode()
		{
			return Col.GetHashCode() + Row.GetHashCode();
		}
	}

	public class Hex
	{
		public int Q { get; set; }
		public int R { get; set; }

		public Hex(int q, int r)
		{
			Q = q;
			R = r;
		}
	}
}
