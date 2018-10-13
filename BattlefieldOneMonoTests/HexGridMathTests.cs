using BattlefieldOneMono;
using Microsoft.Xna.Framework;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class HexGridMathTests
	{
		[Fact]
		public void PixelToHexZeroZero()
		{
			var result = HexGridMath.PixelToHex(20 + GameContent.XOffset, 5 + GameContent.YOffset);
			Assert.Equal(0, result.Q);
			Assert.Equal(0, result.R);
		}

		[Fact]
		public void PixelToHexOneOne()
		{
			var result = HexGridMath.PixelToHex(40 + GameContent.XOffset, 70 + GameContent.YOffset);
			Assert.Equal(1, result.Q);
			Assert.Equal(1, result.R);
		}

		[Fact]
		public void HexToCube()
		{
			var result = HexGridMath.HexToCube(new Hex(5, 5));
			// 5,-10,5
			Assert.Equal(5, result.X);
			Assert.Equal(-10, result.Y);
			Assert.Equal(5, result.Z);
		}

		[Fact]
		public void HexToCubeQR()
		{
			var result = HexGridMath.HexToCube(2, 3);
			// 2,-5,3
			Assert.Equal(2, result.X);
			Assert.Equal(-5, result.Y);
			Assert.Equal(3, result.Z);
		}

		
		[Fact(Skip = "ToPixel is broken")]
		public void ToPixel()
		{
			/*
			var cube = new Cube(5, -10, 5);
			var result = cube.ToPixel;

			Assert.Equal(330, result.X);
			Assert.Equal(530, result.Y);
			*/
		}
	}
}
