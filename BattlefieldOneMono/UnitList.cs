using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BattlefieldOneMono.Constants;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public class UnitList : IEnumerable<Unit>, IUnitList
	{
		private List<Unit> _unitList = new List<Unit>();
		public NATIONALITY CurrentTurn { get; set; } // this indicates whos turn it is
		//private ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private IBattleCalculator _battleCalculator;
		private ITerrainMap _terrainMap;

		public Unit IdleUnit { get; set; }

		public UnitList(IBattleCalculator battleCalculator, ITerrainMap terrainMap)
		{
			_battleCalculator = battleCalculator;
			_terrainMap = terrainMap;
		}

		public int TotalGermanUnits
		{
			get
			{
				int liTotal = 0;

				for (int i = 0; i < _unitList.Count; i++)
				{
					if (_unitList[i].Nationality == NATIONALITY.Germany)
					{
						liTotal++;
					}
				}

				return liTotal;
			}
		}

		public Unit FindSelectedUnit()
		{
			foreach (var unit in _unitList)
			{
				if (unit.Selected)
				{
					return (Unit)unit;
				}
			}

			return null;
		}

		public void UnselectUnits()
		{
			// unselect all units
			foreach (var unit in _unitList)
			{
				unit.Selected = false;
				unit.Flash = null;
			}
		}

		public bool IsAUnitSelected
		{
			get
			{
				foreach (var otherUnit in _unitList)
				{
					if (otherUnit.Selected)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void Update(GameTime gameTime)
		{
			foreach (var unit in _unitList)
			{
				unit.Flash?.Update(gameTime);
			}
		}

		public int FindClosestUnassignedUnit(int col, int row, NATIONALITY nationality)
		{
			var liDistance = double.MaxValue; // just make sure this is bigger than the map board
			var liClosestUnit = -1;

			for (var i = 0; i < _unitList.Count; i++)
			{
				if (_unitList[i].Nationality == nationality && _unitList[i].Command == UNITCOMMAND.None)
				{
					if (_unitList[i].Col != col && _unitList[i].Row != row)
					{
						var liTempDistance = BattleFieldOneCommonObjects.Distance(col, row, _unitList[i].Col, _unitList[i].Row);
						if (liTempDistance < liDistance)
						{
							liDistance = liTempDistance;
							liClosestUnit = i;
						}
					}
				}
			}

			return liClosestUnit;
		}

		public int FindClosestUnit(int col, int row, NATIONALITY nationality)
		{
			var liDistance = double.MaxValue; // just make sure this is bigger than the map board
			var liClosestUnit = -1;

			for (var i = 0; i < _unitList.Count; i++)
			{
				if (_unitList[i].Nationality == nationality)
				{
					if (_unitList[i].Col != col && _unitList[i].Row != row)
					{
						var liTempDistance = BattleFieldOneCommonObjects.Distance(col, row, _unitList[i].Col, _unitList[i].Row);
						if (liTempDistance < liDistance)
						{
							liDistance = liTempDistance;
							liClosestUnit = i;
						}
					}
				}
			}

			return liClosestUnit;
		}

		public bool IsMapOccupied(int col, int row)
		{
			foreach (var unit in _unitList)
			{
				if (unit.Col == col && unit.Row == row)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsTurnComplete(NATIONALITY nationality)
		{
			foreach (var unit in _unitList)
			{
				if (unit.Nationality == nationality)
				{
					if (!unit.TurnComplete)
					{
						return false;
					}
				}
			}

			return true;
		}

		public void SetTurn(NATIONALITY nationality)
		{
			CurrentTurn = nationality;

			foreach (var unit in _unitList)
			{
				if (unit.Nationality == nationality)
				{
					unit.NewTurn();
				}
			}
		}

		public Unit FindUnit(int col, int row)
		{
			foreach (var unit in _unitList)
			{
				if (unit.Col == col && unit.Row == row)
				{
					return unit;
				}
			}

			return null;
		}

		public void DestroyUnit(Unit unit)
		{
			for (int i = 0; i < _unitList.Count; i++)
			{
				if (_unitList[i].UnitNumber == unit.UnitNumber)
				{
					_unitList.RemoveAt(i);
					return;
				}
			}
		}

		public void ClearAllFlash()
		{
			foreach (var unit in _unitList)
			{
				unit.Flash = null;
			}
		}

		public void ComputeAttack(Unit unitAttacking, Unit unitDefending)
		{
			switch (_battleCalculator.Result(unitAttacking.Offense))
			{
				case BATTLERESULT.DefenderDestroyed:
					//_log.Debug($"ComputeAttack: DefenderDestroyed ({unitAttacking.UnitNumber})");
					DestroyUnit(unitAttacking);
					break;
				case BATTLERESULT.EnemyDestroyed:
					DestroyUnit(unitDefending);
					//_log.Debug($"ComputeAttack: EnemyDestroyed ({unitDefending.UnitNumber})");
					break;
				case BATTLERESULT.EnemyDamaged:
					unitDefending.Defense--;
					if (unitDefending.Defense <= 0)
					{
						//_log.Debug($"ComputeAttack: Enemy damaged, EnemyDestroyed ({unitDefending.UnitNumber})");
						DestroyUnit(unitDefending);
					}
					break;
				case BATTLERESULT.EnemyDoubleDamaged:
					unitDefending.Defense -= 2;
					if (unitDefending.Defense <= 0)
					{
						//_log.Debug($"ComputeAttack: Enemy double damanged, EnemyDestroyed ({unitDefending.UnitNumber})");
						DestroyUnit(unitDefending);
					}
					break;
				case BATTLERESULT.EnemyTripleDamaged:
					unitDefending.Defense -= 3;
					if (unitDefending.Defense <= 0)
					{
						//_log.Debug($"ComputeAttack: Enemy triple damaged, EnemyDestroyed ({unitDefending.UnitNumber})");
						DestroyUnit(unitDefending);
					}
					break;
				case BATTLERESULT.None:
					break;
			}
		}

		public Unit FindNextUnit(Unit previousUnit, NATIONALITY nationality)
		{
			if (previousUnit == null)
			{
				// find the first available unit in the units list
				foreach (var currentUnit in _unitList)
				{
					if (currentUnit.Nationality == nationality)
					{
						if (currentUnit.TurnComplete == false)
						{
							return currentUnit;
						}
					}
				}
			}
			else
			{
				// find the next unit in the units list
				int currentUnit = -1;
				for (int i = 0; i < _unitList.Count; i++)
				{
					if (_unitList[i].UnitNumber == previousUnit.UnitNumber)
					{
						currentUnit = i;
						break;
					}
				}

				if (currentUnit > -1)
				{
					for (int i = currentUnit + 1; i < _unitList.Count; i++)
					{
						if (_unitList[i].Nationality == nationality)
						{
							if (_unitList[i].TurnComplete == false)
							{
								return _unitList[i];
							}
						}
					}
				}
			}

			return null;
		}

		public void SetCurrentUnitSleep()
		{
			var unit = FindSelectedUnit();

			if (unit != null)
			{
				//_log.Debug("allied unit:" + unit.UnitNumber + " sleep");
				unit.Sleep = true;
			}
			else if (IdleUnit != null)
			{
				//_log.Debug("allied unit:" + IdleUnit.UnitNumber + " sleep");
				IdleUnit.Sleep = true;
			}
		}

		public void Add(int col, int row, int unitType, NATIONALITY nationality)
		{
			using (var scope = IocContainer.Container.BeginLifetimeScope())
			{
				var unit = scope.Resolve<IUnit>();
				unit.Row = row;
				unit.Col = col;
				unit.UnitType = unitType;
				unit.Nationality = nationality;
				_unitList.Add((Unit)unit);
			}
		}

		public Unit this[int index] => _unitList[index];

		public int Count => _unitList.Count;

		public IEnumerator<Unit> GetEnumerator()
		{
			return _unitList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void DumpData()
		{
			// dump all the unit data
			foreach (var unit in _unitList)
			{
				unit.DumpData();
				//_log.Debug("------------------------------------------------------");
			}
		}

		public void SetNextUnit()
		{
			var unit = FindSelectedUnit();
			if (unit != null)
			{
				unit.Flash = null;
				unit.SkipTurn = true;
			}
			else if (IdleUnit != null)
			{
				IdleUnit.SkipTurn = true;
			}
		}

		public void PlayerCheckForPossibleAttack(Unit unit, IAlliedEnemyMatrix alliedEnemyMatrix)
		{
			// check to see if this unit can attack an enemy unit (in range, unit has not attacked this turn, etc.)
			// if unit cannot attack, then mark as turn complete
			var cells = _terrainMap.FindAdjacentCells(unit.Col, unit.Row, unit.UnitType, unit.Range);
			foreach (var cell in cells)
			{
				var nearbyUnit = FindUnit(cell.Col, cell.Row);
				if (nearbyUnit != null && alliedEnemyMatrix.AreEnemies(nearbyUnit.Nationality, unit.Nationality))
				{
					// this unit can attack a nearby enemy.
					nearbyUnit.Sleep = false;
					return;
				}
			}

			// no nearby enemies in range, end turn for this unit
			unit.UnitHasAttackedThisTurn = true;
		}

		public bool IsUnitWithinAttackRange(Unit unit, IAlliedEnemyMatrix alliedEnemyMatrix)
		{
			var cells = _terrainMap.FindAdjacentCells(unit.Col, unit.Row, unit.UnitType, unit.Range);
			foreach (var cell in cells)
			{
				var nearbyUnit = FindUnit(cell.Col, cell.Row);
				if (nearbyUnit != null && alliedEnemyMatrix.AreEnemies(nearbyUnit.Nationality, unit.Nationality))
				{
					return true;
				}
			}

			return false;
		}

		public List<Unit> FindAllAdjacentUnits(int col, int row)
		{
			var result = new List<Unit>();

			for (var i = col - 1; i <= col + 1; i++)
			{
				for (var j = row - 1; j <= row + 1; j++)
				{
					if (i == col && j == row) continue;

					var unit = FindUnit(i, j);
					if (unit != null)
					{
						result.Add(unit);
					}
				}
			}

			return result;
		}
	}
}
