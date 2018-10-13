using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class AlliedEnemyMatrixTests
	{
		[Fact]
		public void SetAlliance()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetAlliance(NATIONALITY.USA, NATIONALITY.GreatBritian);

			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.GreatBritian));
			Assert.False(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.Germany));
		}

		[Fact]
		public void SetEnemies()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetEnemies(NATIONALITY.USA, NATIONALITY.Germany);

			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Germany));
		}

		[Fact]
		public void GetAllianceFalse()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetEnemies(NATIONALITY.USA, NATIONALITY.Germany);

			Assert.False(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.Germany)); // test enemies
			Assert.False(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.Russia)); // test none
		}

		[Fact]
		public void GetallianceTrue()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetAlliance(NATIONALITY.USA, NATIONALITY.France);

			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.France)); // test allies
			Assert.False(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.Russia)); // test none
		}

		[Fact]
		public void GetEnemeiesFalse()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetAlliance(NATIONALITY.USA, NATIONALITY.GreatBritian);

			Assert.False(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.GreatBritian)); // test allies
			Assert.False(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Russia)); // test none
		}

		[Fact]
		public void GetEnemiesTrue()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetEnemies(NATIONALITY.USA, NATIONALITY.Japan);

			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Japan)); // test enemies
			Assert.False(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Russia)); // test none
		}

		[Fact]
		public void VerifyDefaultAlliances()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(true);

			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.France));
			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.USA, NATIONALITY.GreatBritian));
			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.France, NATIONALITY.GreatBritian));
			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.Germany, NATIONALITY.Japan));
			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.Germany, NATIONALITY.Italy));
			Assert.True(alliedEnemeyMatrix.AreAllied(NATIONALITY.Japan, NATIONALITY.Italy));

			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Germany));
			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.USA, NATIONALITY.Japan));
			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.GreatBritian, NATIONALITY.Germany));
			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.GreatBritian, NATIONALITY.Japan));
			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.France, NATIONALITY.Germany));
			Assert.True(alliedEnemeyMatrix.AreEnemies(NATIONALITY.France, NATIONALITY.Japan));
		}

		[Fact]
		public void AreAlliedOrEqualAllied()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			alliedEnemeyMatrix.SetAlliance(NATIONALITY.USA, NATIONALITY.GreatBritian);
			Assert.True(alliedEnemeyMatrix.AreAlliedOrEqual(NATIONALITY.USA, NATIONALITY.GreatBritian));
		}

		[Fact]
		public void AreAlliedOrEqualEqual()
		{
			var alliedEnemeyMatrix = new AlliedEnemyMatrix(false);

			Assert.True(alliedEnemeyMatrix.AreAlliedOrEqual(NATIONALITY.USA, NATIONALITY.USA));
		}
	}
}
