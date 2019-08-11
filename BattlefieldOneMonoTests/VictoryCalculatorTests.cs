using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class VictoryCalculatorTests
	{
		
		[Fact]
		public void EnemyCapturesAllCities()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			terrainMap[5, 5].BackgroundType = 40;
			terrainMap[6, 2].BackgroundType = 40;

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(5, 5, 1, NATIONALITY.Germany);
			unitList.Add(6, 2, 1, NATIONALITY.Japan);
			unitList.Add(6, 6, 1, NATIONALITY.GreatBritian);

			var calc = new VictoryCalculator(terrainMap, unitList);
			calc.GermanCaptureCitiesToWin(2);
			calc.AlliesCaptureCitiesToWin(2);

			Assert.Equal("Axis Forces Captured All Cities!", calc.Result());
		}

		[Fact]
		public void AlliesCaptureAllCities()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			terrainMap[5, 5].BackgroundType = 40;
			terrainMap[6, 2].BackgroundType = 40;

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(5, 5, 1, NATIONALITY.France);
			unitList.Add(6, 2, 1, NATIONALITY.GreatBritian);
			unitList.Add(6, 7, 1, NATIONALITY.Germany);

			var calc = new VictoryCalculator(terrainMap, unitList);
			calc.GermanCaptureCitiesToWin(2);
			calc.AlliesCaptureCitiesToWin(2);

			Assert.Equal("Allies Captured All Cities!", calc.Result());
		}

		[Fact]
		public void AllAlliedUnitsDestroyed()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			terrainMap[5, 5].BackgroundType = 40;
			terrainMap[6, 2].BackgroundType = 40;

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(6, 6, 1, NATIONALITY.GreatBritian);

			var calc = new VictoryCalculator(terrainMap, unitList);
			calc.GermanCaptureCitiesToWin(2);
			calc.AlliesCaptureCitiesToWin(2);

			Assert.Equal("All Axis Units Destroyed!", calc.Result());
		}

		[Fact]
		public void AllEnemyUnitsDestroyed()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			terrainMap[5, 5].BackgroundType = 40;
			terrainMap[6, 2].BackgroundType = 40;

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(6, 7, 1, NATIONALITY.Germany);

			var calc = new VictoryCalculator(terrainMap, unitList);
			calc.GermanCaptureCitiesToWin(2);
			calc.AlliesCaptureCitiesToWin(2);

			Assert.Equal("All Allied Units Destroyed!", calc.Result());
		}

        [Fact]
        public void TotalAlliedUnitsAlive()
        {
            IocContainer.Setup();

            var terrainMap = new TerrainMap(new ShortestPath());
            terrainMap.InitializeBoard(8, 8);

            var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
            unitList.Add(6, 7, 1, NATIONALITY.USA);
            unitList.Add(3, 2, 1, NATIONALITY.USA);
            unitList.Add(3, 3, 1, NATIONALITY.Germany);

            var calc = new VictoryCalculator(terrainMap, unitList);

            Assert.Equal(2,calc.TotalAlliedUnitsAlive);
        }

        [Fact]
        public void TotalGermanUnitsAlive()
        {
            IocContainer.Setup();

            var terrainMap = new TerrainMap(new ShortestPath());
            terrainMap.InitializeBoard(8, 8);

            var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
            unitList.Add(6, 7, 1, NATIONALITY.Germany);
            unitList.Add(3, 2, 1, NATIONALITY.Germany);
            unitList.Add(3, 3, 1, NATIONALITY.Germany);
            unitList.Add(1, 1, 1, NATIONALITY.USA);

            var calc = new VictoryCalculator(terrainMap, unitList);

            Assert.Equal(3, calc.TotalGermanUnitsAlive);
        }
    }
}
