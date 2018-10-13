using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class UnitTests
	{
		[Fact]
		public void CompareUnitsEqual()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 3,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};

			var unit2 = unit;

			Assert.True(unit == unit2);
		}

		[Fact]
		public void CompareUnitsEqualIsFalse()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 3,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};

			var unit2 = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};

			Assert.False(unit == unit2);
		}

		[Fact]
		public void CompareUnitsNotEqual()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 3,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};
			var unit2 = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};

			Assert.True(unit != unit2);
		}

		[Fact]
		public void ToCube()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};

			var cube = unit.ToCube;

			Assert.Equal(2, cube.X);
			Assert.Equal(-6, cube.Y);
			Assert.Equal(4, cube.Z);
		}

		[Fact]
		public void IsAtDestination()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA
			};
			unit.DestCol = unit.Col;
			unit.DestRow = unit.Row;

			Assert.True(unit.IsAtDestination);
		}

		[Fact]
		public void NotAtDestination()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 1,
				Nationality = NATIONALITY.USA,
				DestCol = 1,
				DestRow = 1
			};

			Assert.False(unit.IsAtDestination);
		}

		[Fact]
		public void NewTurn()
		{
			var unit = new Unit(new ShortestPath())
			{
				Col = 2,
				Row = 5,
				UnitType = 0,
				Nationality = NATIONALITY.USA,
				Movement = 0,
				UnitHasAttackedThisTurn = true,
				UnitWait = 20
			};

			unit.NewTurn();

			Assert.Equal(1, unit.Movement);
			Assert.False(unit.TurnComplete);
			Assert.Equal(0, unit.UnitWait);
		}


	}
}
