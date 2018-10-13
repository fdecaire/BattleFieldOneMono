using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class TerrainCellTests
	{
		[Fact]
		public void CellIsACity()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 40);
			Assert.True(terrainCell.IsCity);
		}

		[Fact]
		public void CellIsNotACity()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 10);
			Assert.False(terrainCell.IsCity);
		}

		[Fact]
		public void TerrainDoubledOnRoad()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 0);
			terrainCell.Roads = RoadType.Road1;

			Assert.Equal(0.5,terrainCell.GroundUnitTerrainModifier);
		}

		[Fact]
		public void TerrainDoubledOnCity()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 40);

			Assert.Equal(0.5, terrainCell.GroundUnitTerrainModifier);
		}

		[Fact]
		public void TerrainNormalOnGrass()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 0);

			Assert.Equal(1, terrainCell.GroundUnitTerrainModifier);
		}

		[Fact]
		public void TerrainBlockedOnWaterForTank()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 20);
			Assert.True(terrainCell.Blocked(1));
		}

		[Fact]
		public void TerrainNoBlockedOnGrassForUnit()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 0);
			Assert.False(terrainCell.Blocked(0));
		}
		[Fact]
		public void TerrainBlockedOnForrestForTank()
		{
			var terrainCell = new TerrainCell(0, 0, 0, 50);
			Assert.True(terrainCell.Blocked(1));
		}
	}
}
