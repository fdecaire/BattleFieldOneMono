using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class ShortestPathTests
	{
		[Fact(Skip = "Needs work")]
		public void FindShortestPath()
		{
			var shortest = new ShortestPath();
			var map = new TerrainMap(shortest);
			map.InitializeBoard(10, 10);

			//mountains 4,1 - 4,8
			for (var i = 1; i < 9; i++)
			{
				var cube = HexGridMath.OffsetToCube(4, i);
				map[4, i] = new TerrainCell(cube.X, cube.Y, cube.Z, 30);
			}

			// 1,3 to 8,4
			shortest.ComputeShortestPath(map, 1, 3, 8, 4, 0);

			Assert.Equal(new Offset(2, 3), shortest.WayPoint[0]);
			Assert.Equal(new Offset(3, 2), shortest.WayPoint[1]);
			Assert.Equal(new Offset(3, 1), shortest.WayPoint[2]);
			Assert.Equal(new Offset(3, 0), shortest.WayPoint[3]);
			Assert.Equal(new Offset(4, 0), shortest.WayPoint[4]);
			Assert.Equal(new Offset(5, 0), shortest.WayPoint[5]);
			Assert.Equal(new Offset(6, 1), shortest.WayPoint[6]);
			Assert.Equal(new Offset(6, 2), shortest.WayPoint[7]);
			Assert.Equal(new Offset(6, 3), shortest.WayPoint[8]);
			Assert.Equal(new Offset(7, 3), shortest.WayPoint[9]);
			Assert.Equal(new Offset(8, 4), shortest.WayPoint[10]);
		}

		[Fact]
		public void UseRoads()
		{
			//https://www.redblobgames.com/pathfinding/a-star/introduction.html
			var shortest = new ShortestPath();
			var map = new TerrainMap(shortest);
			map.InitializeBoard(6, 6);

			map[1, 2].Roads = RoadType.Road2 | RoadType.Road5;
			map[2, 3].Roads = RoadType.Road2 | RoadType.Road5;
			map[3, 3].Roads = RoadType.Road3 | RoadType.Road5;
			map[4, 3].Roads = RoadType.Road2 | RoadType.Road6;
			map[5, 3].Roads = RoadType.Road5 | RoadType.Road1;
			map[5, 4].Roads = RoadType.Road1 | RoadType.Road4;
			map[5, 5].Roads = RoadType.Road4;

            shortest.ComputeShortestPath(map, 1, 2, 5, 5, 0);

			Assert.Equal(new Offset(2, 3), shortest.WayPoint[0]);
			Assert.Equal(new Offset(3, 3), shortest.WayPoint[1]);
			Assert.Equal(new Offset(4, 3), shortest.WayPoint[2]);
			Assert.Equal(new Offset(5, 3), shortest.WayPoint[3]);
			Assert.Equal(new Offset(5, 4), shortest.WayPoint[4]);
			Assert.Equal(new Offset(5, 5), shortest.WayPoint[5]);
        }
	}
}
