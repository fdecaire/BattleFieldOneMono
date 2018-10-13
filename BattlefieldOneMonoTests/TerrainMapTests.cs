using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class TerrainMapTests
	{
		[Fact]
		public void TestBoardInitialize()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(10, 20);

			Assert.Equal(10, map.Width);
			Assert.Equal(20, map.Height);
		}

		[Fact]
		public void TestGBoardResize()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(2, 2);

			map.InitializeBoard(15, 10);

			Assert.Equal(15, map.Width);
			Assert.Equal(10, map.Height);
		}

		[Fact]
		public void CityListCount()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			map[2, 3] = new TerrainCell(2, 3, 40);
			map[4, 4] = new TerrainCell(4, 4, 40);

			Assert.Equal(2, map.TotalCities);
		}

		[Fact]
		public void CityList()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			map[2, 3] = new TerrainCell(2, 3, 40);
			map[4, 4] = new TerrainCell(4, 4, 40);
			map[4, 2] = new TerrainCell(4, 2, 40);

			Assert.Equal(3, map.TotalCities);
		}

		[Fact]
		public void ClearMask()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(4, 5);
			map.ClearMask();

			for (var y=0;y<5;y++)
			{
				for (var x=0;x<4;x++)
				{
					Assert.False(map[x, y].Mask);
				}
			}
		}

		[Fact]
		public void UnmaskBoard()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(5, 5);
			map.UnmaskBoard(2,2, 1);

			var resultArray = new bool[5, 5] {
				{ true, true, true, true, true },
				{ true, false, false, false, true},
				{ true, false, false, false, true},
				{ true, true, false, true, true },
				{ true, true, true, true, true }
			};

			for (var y = 0; y < 5; y++)
			{
				for (var x = 0; x < 5; x++)
				{
					Assert.Equal(resultArray[y, x], map[x, y].Mask);
				}
			}
		}

		[Fact]
		public void SetView()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(5, 5);
			map.SetView(2, 2, 1);

			var resultArray = new bool[5, 5] {
				{ true, true, true, true, true },
				{ true, false, false, false, true},
				{ true, false, false, false, true},
				{ true, true, false, true, true },
				{ true, true, true, true, true }
			};

			for (var y = 0; y < 5; y++)
			{
				for (var x = 0; x < 5; x++)
				{
					Assert.Equal(!resultArray[y, x], map[x, y].View);
				}
			}
		}

		[Fact]
		public void FindClosestCityNoCities()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(5, 5);

			Assert.Null(map.FindClosestCity(1, 1));
		}

		[Fact]
		public void FindClosestCity()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(10, 10);
			map[5, 5] = new TerrainCell(5, -10, 5, 40);
			map[7, 6] = new TerrainCell(7, -13, 6, 40);

			var result = map.FindClosestCity(1, 1);
			Assert.Equal(5, result.Col);
			Assert.Equal(5, result.Row);
		}

		[Fact]
		public void FindAdjacentCellsRange1()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(10, 10);

			var result = map.FindAdjacentCells(3, 3, 0, 1);

			Assert.Equal(6, result.Count);
			Assert.Equal(2, result[0].Col);
			Assert.Equal(4, result[0].Row);

			Assert.Equal(2, result[1].Col);
			Assert.Equal(3, result[1].Row);

			Assert.Equal(3, result[2].Col);
			Assert.Equal(4, result[2].Row);

			Assert.Equal(3, result[3].Col);
			Assert.Equal(2, result[3].Row);

			Assert.Equal(4, result[4].Col);
			Assert.Equal(4, result[4].Row);

			Assert.Equal(4, result[5].Col);
			Assert.Equal(3, result[5].Row);
		}

		[Fact]
		public void FindAdjacentCellsRange2()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(10, 10);

			var result = map.FindAdjacentCells(3, 3, 1, 2);

			Assert.Equal(18, result.Count);
		}

		[Fact]
		public void IndexerTest()
		{
			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap[2, 2] = new TerrainCell(0, 0, 0, 2);

			Assert.Equal(2, terrainMap[2, 2].BackgroundType);
		}

		[Fact]
		public void HasherTest()
		{
			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap[2, 2] = new TerrainCell(0, 0, 0, 2);
			var cube = HexGridMath.OffsetToCube(2, 2);
			var hash = terrainMap.FindMapHash(cube.X,cube.Y,cube.Z);

			var terrain = terrainMap[hash];
			Assert.Equal(2,terrain.BackgroundType);
		}
	}
}
