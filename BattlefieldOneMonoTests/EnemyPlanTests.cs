using BattlefieldOneMono;
using Xunit;

namespace BattlefieldOneMonoTests
{
	public class EnemyPlanTests
	{
		//TODO: need to fix this
		
		[Fact(Skip="broken test")]
		public void all_enemy_units_stuck()
		{
			IocContainer.Setup();

			var map = new TerrainMap(new ShortestPath());
			var units = new UnitList(new BattleCalculator(new DieRoller()),new TerrainMap(new ShortestPath()))
			{
				{1, 1, 1, NATIONALITY.Germany},
				{1, 1, 1, NATIONALITY.Germany},
				{1, 1, 1, NATIONALITY.Germany}
			};

			units[0].Sleep = true;
			units[1].SkipTurn = true;
			units[2].Sleep = true;

			var alliedEnemyMatrix = new AlliedEnemyMatrix(true);

			var enemyPlan = new EnemyPlan(map, units, alliedEnemyMatrix);

			Assert.True(enemyPlan.AreAllEnemyUnitsStuck());
		}
		
	}
}
