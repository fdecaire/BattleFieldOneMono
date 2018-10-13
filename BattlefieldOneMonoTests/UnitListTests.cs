using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class UnitListTests
	{
		[Fact]
		public void FindUnit()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			var unit = units.FindUnit(3, 3);

			Assert.Equal(NATIONALITY.USA, unit.Nationality);
		}

		[Fact]
		public void SetAlliedTurn()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			units[0].Movement = 0;
			units[1].Movement = 0;
			units[2].Movement = 0;
			units.SetTurn(NATIONALITY.USA);

			Assert.Equal(0, units[0].Movement);
			Assert.Equal(1, units[1].Movement);
			Assert.Equal(0, units[2].Movement);
		}

		[Fact]
		public void IsTurnCompleteGermainYes()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);
			units[0].Movement = 0;
			units[0].UnitHasAttackedThisTurn = true;
			units[2].Movement = 0;
			units[2].UnitHasAttackedThisTurn = true;

			Assert.True(units.IsTurnComplete(NATIONALITY.Germany));
		}

		[Fact]
		public void IsTurnCompleteGermainNotMoved()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);
			units[0].Movement = 0;

			Assert.False(units.IsTurnComplete(NATIONALITY.Germany));
		}

		[Fact]
		public void IsTurnCompleteAlliedYes()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);
			units[1].Movement = 0;
			units[1].UnitHasAttackedThisTurn = true;

			Assert.True(units.IsTurnComplete(NATIONALITY.USA));
		}

		[Fact]
		public void IsTurnCompleteAlliedNotMoved()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			Assert.False(units.IsTurnComplete(NATIONALITY.USA));
		}

		[Fact]
		public void IsTurnCompleteAlliedNotFired()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);
			units[1].Movement = 0;

			Assert.False(units.IsTurnComplete(NATIONALITY.USA));
		}

		[Fact]
		public void IsMapOccupiedOffset()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			Assert.True(units.IsMapOccupied(2, 2));
			Assert.False(units.IsMapOccupied(1, 1));
		}

		[Fact]
		public void FindClosestGermanUnitOffset()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			int closestUnit = units.FindClosestUnit(1, 1, NATIONALITY.Germany);

			Assert.Equal(0, closestUnit);
		}

		[Fact]
		public void FindClosestGermanUnitOffsetNoGermanUnits()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			int closestUnit = units.FindClosestUnit(1, 1, NATIONALITY.Germany);

			Assert.Equal(-1, closestUnit);
		}

		[Fact]
		public void IsAUnitSelectedNo()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			Assert.False(units.IsAUnitSelected);
		}

		[Fact]
		public void IsAUnitSelectedYes()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			units[1].Selected = true;

			Assert.True(units.IsAUnitSelected);
		}

		[Fact]
		public void UnselectUnits()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			units[1].Selected = true;

			units.UnselectUnits();

			for (int i = 0; i < 3; i++)
			{
				Assert.False(units[i].Selected);
			}
		}

		[Fact]
		public void FindSelectedUnit()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			units[1].Selected = true;

			Assert.Equal(units[1], units.FindSelectedUnit());
		}

		[Fact]
		public void TotalGermanUnits()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			Assert.Equal(2, units.TotalGermanUnits);
		}

		[Fact]
		public void TotalGermanUnitsNone()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			Assert.Equal(0, units.TotalGermanUnits);
		}

		[Fact]
		public void TotalGermanUnitsNoUnits()
		{
			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			Assert.Equal(0, units.TotalGermanUnits);
		}

		[Fact]
		public void DestroyUnit()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			Unit unit = units[1];
			units.DestroyUnit(unit);

			Assert.Equal(2, units.Count);
			Assert.Equal(2, units[0].Col);
			Assert.Equal(2, units[0].Row);
			Assert.Equal(4, units[1].Col);
			Assert.Equal(4, units[1].Row);
		}

		[Fact]
		public void DestroyUnitEmptyList()
		{
			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			var unit = new Unit(new ShortestPath())
			{
				Col = 5,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};
			units.DestroyUnit(unit);

			Assert.Equal(0, units.Count);
		}

		[Fact]
		public void DestroyUnitNullUnit()
		{
			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.DestroyUnit(null);

			Assert.Equal(0, units.Count);
		}

		[Fact]
		public void FindNextUnitNoPreviousUnit()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			var unit = units.FindNextUnit(null, NATIONALITY.Germany);

			Assert.Equal(units[0].UnitNumber, unit.UnitNumber);
		}

		[Fact]
		public void FindNextUnitWithPreviousUnit()
		{
			IocContainer.Setup();

			var units = new UnitList(new BattleCalculator(new DieRoller()),new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			var unit = units.FindNextUnit(units[0], NATIONALITY.Germany);

			Assert.Equal(units[2].UnitNumber, unit.UnitNumber);
		}

		[Fact]
		public void FindNextUnitSkipUnitWithZeroMoves()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.Germany);
			units.Add(4, 5, 0, NATIONALITY.Germany);

			units[2].Movement = 0;
			units[2].UnitHasAttackedThisTurn = true;

			var unit = units.FindNextUnit(units[0], NATIONALITY.Germany);

			Assert.Equal(units[3].UnitNumber, unit.UnitNumber);
		}

		[Fact]
		public void FindNextUnitNoUnitFound()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.Germany);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			var unit = units.FindNextUnit(units[0], NATIONALITY.USA);

			Assert.Null(unit);
		}

		[Fact]
		public void FindNextUnitSkipSleepingUnit()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.USA);
			units.Add(3, 3, 0, NATIONALITY.USA);
			units.Add(4, 4, 0, NATIONALITY.USA);

			units[0].Sleep = true;
			units[1].Sleep = true;

			var unit = units.FindNextUnit(units[0], NATIONALITY.USA);

			Assert.Equal(units[2], unit);
		}

		[Fact]
		public void UnitListCountZero()
		{
			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			Assert.Equal(0, units.Count);
		}

		[Fact]
		public void UnitListCountGreaterThanZero()
		{
			IocContainer.Setup();

			UnitList units = new UnitList(new BattleCalculator(new DieRoller()), new TerrainMap(new ShortestPath()));

			units.Add(2, 2, 0, NATIONALITY.Germany);
			units.Add(3, 3, 0, NATIONALITY.Germany);
			units.Add(4, 4, 0, NATIONALITY.Germany);

			Assert.Equal(3, units.Count);
		}

		[Fact]
		public void FindAllAdjacentUnitsMiddle()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(3, 3, 1, NATIONALITY.GreatBritian);
			unitList.Add(3, 4, 1, NATIONALITY.France);
			unitList.Add(3, 5, 1, NATIONALITY.France);
			unitList.Add(4, 3, 1, NATIONALITY.France);
			unitList.Add(4, 5, 1, NATIONALITY.Germany);
			unitList.Add(5, 3, 1, NATIONALITY.USA);
			unitList.Add(5, 4, 1, NATIONALITY.France);
			unitList.Add(5, 5, 1, NATIONALITY.Japan);

			var result = unitList.FindAllAdjacentUnits(4, 4);
			Assert.Equal(8, result.Count);
		}

		[Fact]
		public void FindAllAdjacentUnitsEdge()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(1, 3, 1, NATIONALITY.GreatBritian);
			unitList.Add(2, 3, 1, NATIONALITY.France);
			unitList.Add(2, 4, 1, NATIONALITY.France);
			unitList.Add(1, 5, 1, NATIONALITY.France);
			unitList.Add(2, 5, 1, NATIONALITY.Germany);

			var result = unitList.FindAllAdjacentUnits(1, 4);
			Assert.Equal(5, result.Count);
		}
		[Fact]
		public void FindAllAdjacentUnitsCorner()
		{
			IocContainer.Setup();

			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(2, 1, 1, NATIONALITY.GreatBritian);
			unitList.Add(1, 2, 1, NATIONALITY.France);
			unitList.Add(2, 2, 1, NATIONALITY.France);

			var result = unitList.FindAllAdjacentUnits(1, 1);
			Assert.Equal(3, result.Count);
		}

		[Fact]
		public void UnitIsInAttackRange()
		{
			IocContainer.Setup();

			var alliedEnemyMatrix = new AlliedEnemyMatrix(true);
			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(0, 0, 0, NATIONALITY.GreatBritian);
			unitList.Add(2, 2, 2, NATIONALITY.Germany);

			var result = unitList.IsUnitWithinAttackRange(unitList[1], alliedEnemyMatrix);
			Assert.True(result);
		}

		[Fact]
		public void UnitIsNotInAttackRange()
		{
			IocContainer.Setup();

			var alliedEnemyMatrix = new AlliedEnemyMatrix(true);
			var terrainMap = new TerrainMap(new ShortestPath());
			terrainMap.InitializeBoard(8, 8);

			var unitList = new UnitList(new BattleCalculator(new DieRoller()), terrainMap);
			unitList.Add(0, 0, 0, NATIONALITY.GreatBritian);
			unitList.Add(5, 5, 2, NATIONALITY.Germany);

			var result = unitList.IsUnitWithinAttackRange(unitList[1], alliedEnemyMatrix);
			Assert.False(result);
		}
	}
}
