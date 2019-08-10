using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono.Interfaces
{
	public interface IUnitList
	{
		Unit this[int index] { get; }

		int Count { get; }
		NATIONALITY CurrentTurn { get; set; }
		Unit IdleUnit { get; set; }
		bool IsAUnitSelected { get; }
		int TotalGermanUnits { get; }

		void Update(GameTime gameTime);
		void Add(int col, int row, int unitType, NATIONALITY nationality);
		void ComputeAttack(Unit unitAttacking, Unit unitDefending);
		void DestroyUnit(Unit unit);
		void DumpData();
		int FindClosestUnassignedUnit(int col, int row, NATIONALITY nationality);
		int FindClosestUnit(int col, int row, NATIONALITY nationality);
		Unit FindNextUnit(Unit previousUnit, NATIONALITY nationality);
		Unit FindSelectedUnit();
		Unit FindUnit(int col, int row);
		IEnumerator<Unit> GetEnumerator();
		bool IsMapOccupied(int col, int row);
		bool IsTurnComplete(NATIONALITY nationality);
		void PlayerCheckForPossibleAttack(Unit unit, IAlliedEnemyMatrix alliedEnemyMatrix);
		void SetNextUnit();
		void ClearAllFlash();
		void SetTurn(NATIONALITY nationality);
		void UnselectUnits();
		void SetCurrentUnitSleep();
		List<Unit> FindAllAdjacentUnits(int col, int row);
		bool IsUnitWithinAttackRange(Unit unit, IAlliedEnemyMatrix alliedEnemyMatrix);
	}
}