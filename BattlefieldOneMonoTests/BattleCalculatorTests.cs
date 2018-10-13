using BattlefieldOneMono;
using BattlefieldOneMono.Constants;
using BattlefieldOneXNATests;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class BattleCalculatorTests
	{
		[Fact]
		public void CalculateEnemyDamaged()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 2;
			fakeDieRoller.SetDieRoll = 2;
			fakeDieRoller.SetDieRoll = 3;

			var battleCalculator = new BattleCalculator(fakeDieRoller);
			Assert.Equal(BATTLERESULT.EnemyDamaged, battleCalculator.Result(1));
		}

		[Fact]
		public void CalculateEnemyDoubleDamaged()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 3;
			fakeDieRoller.SetDieRoll = 3;
			fakeDieRoller.SetDieRoll = 3;

			// offense > 1
			var battleCalculator = new BattleCalculator(fakeDieRoller);
			Assert.Equal(BATTLERESULT.EnemyDoubleDamaged, battleCalculator.Result(2));
		}

		[Fact]
		public void CalculateEnemyTripleDamaged()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 5;
			fakeDieRoller.SetDieRoll = 5;
			fakeDieRoller.SetDieRoll = 5;

			// offense > 12
			var battleCalculator = new BattleCalculator(fakeDieRoller);
			Assert.Equal(BATTLERESULT.EnemyTripleDamaged, battleCalculator.Result(13));
		}

		[Fact]
		public void CalculateNoDamage()
		{
			var fakeDieRoller = new FakeDieRoller();
			fakeDieRoller.ClearDieRoll();
			fakeDieRoller.SetDieRoll = 1;
			fakeDieRoller.SetDieRoll = 1;
			fakeDieRoller.SetDieRoll = 1;

			// offense > 12
			var battleCalculator = new BattleCalculator(fakeDieRoller);
			Assert.Equal(BATTLERESULT.None, battleCalculator.Result(1));
		}

	}
}
