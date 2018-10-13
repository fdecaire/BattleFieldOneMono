using BattlefieldOneMono.Constants;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public interface IEnemyPlan
	{
		Unit NextEnemyUnitToMove { get; set; }
		MapCoordinates NextEnemyUnitMoveCoords { get; set; }
		Unit EnemyUnitAttacking { get; set; }
		Unit AlliedUnitUnderAttack { get; set; }
		int EnemyAttackFlash { get; set; }
		void StartEnemyTurn();
		void HandleEnemy(GameTime gameTime, bool visible);
		void FindEnemyUnitToAttackFrom();
		void SetEnemyStrategy(STRATEGY strategy);
		bool AreAllEnemyUnitsStuck();
	}
}
