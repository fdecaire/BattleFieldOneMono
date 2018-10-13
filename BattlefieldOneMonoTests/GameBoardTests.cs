using System.Linq;
using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class GameBoardTests
	{
		[Fact]
		public void TestViewMapRegion()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(10, 20);
			map.SetView(3, 3, 1);

			Assert.True(map[3, 3].View); //center
			Assert.True(map[2, 3].View); //left
			Assert.True(map[3, 2].View); //above
			Assert.True(map[4, 3].View); //right
			Assert.True(map[3, 4].View); //below

			for (int x = 0; x < 10; x++)
			{
				for (int y = 0; y < 20; y++)
				{
					if (x == 3 && y == 3)
					{
						continue;
					}
					if (x == 2 && y == 3)
					{
						continue;
					}
					if (x == 3 && y == 2)
					{
						continue;
					}
					if (x == 4 && y == 3)
					{
						continue;
					}
					if (x == 3 && y == 4)
					{
						continue;
					}
					Assert.False(map[2, 2].View);
				}
			}
		}

		[Fact]
		public void FindAdjacentCellsTopLeftCornerTest()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				map[3, i + 1] = new TerrainCell(3, i + 1, 30);
			}

			var surroundingCells = map.FindAdjacentCells(0, 0, 2, 1)
				.OrderBy(u => u.Col)
				.ThenBy(u => u.Row)
				.ToList();

			Assert.Equal(2, surroundingCells.Count);
			Assert.Equal(0, surroundingCells[0].Col);
			Assert.Equal(1, surroundingCells[0].Row);
			Assert.Equal(1, surroundingCells[1].Col);
			Assert.Equal(0, surroundingCells[1].Row);
		}

		[Fact]
		public void FindAdjacentCellsLeftSideTest()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				map[3, i + 1] = new TerrainCell(3, i + 1, 30);
			}

			var surroundingCells = map.FindAdjacentCells(0, 3, 2, 1)
				.OrderBy(u => u.Col)
				.ThenBy(u => u.Row)
				.ToList();

			Assert.Equal(4, surroundingCells.Count);
			Assert.Equal(0, surroundingCells[0].Col);
			Assert.Equal(2, surroundingCells[0].Row);
			Assert.Equal(0, surroundingCells[1].Col);
			Assert.Equal(4, surroundingCells[1].Row);
			Assert.Equal(1, surroundingCells[2].Col);
			Assert.Equal(2, surroundingCells[2].Row);
			Assert.Equal(1, surroundingCells[3].Col);
			Assert.Equal(3, surroundingCells[3].Row);
		}

		[Fact]
		public void FindAdjacentCellsNearUnpassibleTerrainTest()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				map[3, i + 1] = new TerrainCell(3, i + 1, 30);
			}

			var surroundingCells = map.FindAdjacentCells(2, 3, 2, 1)
				.OrderBy(u => u.Col)
				.ThenBy(u => u.Row)
				.ToList();

			Assert.Equal(4, surroundingCells.Count);
			Assert.Equal(1, surroundingCells[0].Col);
			Assert.Equal(2, surroundingCells[0].Row);
			Assert.Equal(1, surroundingCells[1].Col);
			Assert.Equal(3, surroundingCells[1].Row);
			Assert.Equal(2, surroundingCells[2].Col);
			Assert.Equal(2, surroundingCells[2].Row);
			Assert.Equal(2, surroundingCells[3].Col);
			Assert.Equal(4, surroundingCells[3].Row);
		}

		[Fact]
		public void FindAdjacentCellsCenterMapTest()
		{
			var map = new TerrainMap(new ShortestPath());
			map.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				map[3, i + 1] = new TerrainCell(3, i + 1, 30);
			}

			var surroundingCells = map.FindAdjacentCells(1, 3, 2, 1)
				.OrderBy(u => u.Col)
				.ThenBy(u => u.Row)
				.ToList();

			Assert.Equal(6, surroundingCells.Count);
			Assert.Equal(0, surroundingCells[0].Col);
			Assert.Equal(3, surroundingCells[0].Row);
			Assert.Equal(0, surroundingCells[1].Col);
			Assert.Equal(4, surroundingCells[1].Row);
			Assert.Equal(1, surroundingCells[2].Col);
			Assert.Equal(2, surroundingCells[2].Row);
			Assert.Equal(1, surroundingCells[3].Col);
			Assert.Equal(4, surroundingCells[3].Row);
			Assert.Equal(2, surroundingCells[4].Col);
			Assert.Equal(3, surroundingCells[4].Row);
			Assert.Equal(2, surroundingCells[5].Col);
			Assert.Equal(4, surroundingCells[5].Row);
		}
	}
}
