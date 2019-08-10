using System.Collections.Generic;
using BattlefieldOneMono.Constants;
using BattlefieldOneMono.Interfaces;
using Microsoft.Xna.Framework;
using NLog;

namespace BattlefieldOneMono
{
	public class EnemyPlan : IEnemyPlan
	{
		public Unit NextEnemyUnitToMove { get; set; }
		public MapCoordinates NextEnemyUnitMoveCoords { get; set; }
		public readonly Queue<Unit> EnemyUnitQueue = new Queue<Unit>();
		public Unit EnemyUnitAttacking { get; set; }
		public Unit AlliedUnitUnderAttack { get; set; }
		public int EnemyAttackFlash { get; set; }
		private readonly ITerrainMap _map;
		private readonly IUnitList _units;
		private readonly IAlliedEnemyMatrix _alliedEnemyMatrix;
		private int _totalUnitsMovedOrAttacked = 1;
		private STRATEGY _currentStrategy;
		private readonly Logger _log = LogManager.GetCurrentClassLogger();

		private int TotalCitiesCaptured
		{
			get
			{
				int total = 0;
				foreach (var unit in _units)
				{
					if (_map[unit.Col, unit.Row].IsCity)
					{
						total++;
					}
				}

				return total;
			}
		}
		private bool IsUnitFlashing(bool noMaskView)
		{
			if (!noMaskView)
			{
				return false;
			}

			if (EnemyUnitAttacking != null && EnemyUnitAttacking.Flash !=null && !EnemyUnitAttacking.Flash.Complete)
			{
				return true;
			}

			if (NextEnemyUnitToMove != null && NextEnemyUnitToMove.Flash != null && !NextEnemyUnitToMove.Flash.Complete)
			{
				return true;
			}

			return false;
		}

		public EnemyPlan(ITerrainMap map, IUnitList units, IAlliedEnemyMatrix alliedEnemyMatrix)
		{
			_map = map;
			_units = units;
			_alliedEnemyMatrix = alliedEnemyMatrix;
		}

		public void StartEnemyTurn()
		{
			//_log.Debug("");
			//_log.Debug("StartEnemyTurn()");

			NextEnemyUnitToMove = null;

			foreach (var unit in _units)
			{
				//_log.Debug("Unit " + unit.UnitNumber + " " + unit.Col + "," + unit.Row + " " + unit.Nationality);

				// if this is an enemy of the player...
				if (_alliedEnemyMatrix.AreEnemies(unit.Nationality, NATIONALITY.USA))
				{
					if (unit.TurnComplete == false)
					{
						// stuff these units into a queue
						EnemyUnitQueue.Enqueue(unit);
					}
				}
			}

			//TODO: detect if the enemy units are all stuck and we need to switch strategies
			if (_currentStrategy == STRATEGY.CaptureAllCities)
			{
				//_log.Debug("StartEnemyTurn() capture all cities employed");
				//if (_totalUnitsMovedOrAttacked == 0)
				if (TotalCitiesCaptured > 0)
				{
					//TODO: ignore for first move
					SetEnemyStrategy(STRATEGY.AttackAllUnits);
				}
			}

			_totalUnitsMovedOrAttacked = 0;
		}

		public void HandleEnemy(GameTime gameTime, bool visible)
		{
			if (_units.CurrentTurn == NATIONALITY.Germany)
			{
				//TODO: need to detect when units are blocked or all idle.

				if (NextEnemyUnitToMove == null && EnemyUnitAttacking == null)
				{
					//_log.Debug("HandleEnemy() NextEnemyToMove = null");
					// find another unit to move around the map
					NextEnemyUnitToMove = FindNextUnitToMove(gameTime);

					if (NextEnemyUnitToMove == null)
					{
						//_log.Debug("HandleEnemy() NextEnemyToMove is still null");
					}
					else
					{
						//_log.Debug("HandleEnemy() NextEnemyToMove=" + NextEnemyUnitToMove.Nationality);
						_log.Debug($"HandleEnemyUnitMove() set unit flash [unit:{NextEnemyUnitToMove.UnitNumber}]");
						NextEnemyUnitToMove.Flash = new UnitFlash(2, 0.2f, new Color(100, 200, 100));
					}
				}

				// this will move the enemy unit one space at a time
				if (NextEnemyUnitToMove != null)
				{
					HandleEnemyUnitMove(visible);
				}

				// forallunits((Movement == 0 && UnitHasAttackedThisTurn) || Sleep || SkipTurn)
				if (_units.IsTurnComplete(NATIONALITY.Germany) || EnemyUnitQueue.Count == 0)
				{
					//_log.Debug("HandleEnemy() turn complete");

					_units.SetTurn(NATIONALITY.USA);
					EnemyUnitAttacking = null;
					AlliedUnitUnderAttack = null;
					NextEnemyUnitToMove = null;
				}
			}
		}

		private void HandleEnemyUnitMove(bool visible)
		{
			if (IsUnitFlashing(visible))
			{
				return;
			}

			_log.Debug($"HandleEnemyUnitMove() set new coords from:{NextEnemyUnitToMove.Coordinates} movement={NextEnemyUnitToMove.Movement} [unit:{NextEnemyUnitToMove.UnitNumber}]");

			if (NextEnemyUnitMoveCoords != null)
			{
				if (_units.IsUnitWithinAttackRange(NextEnemyUnitToMove, _alliedEnemyMatrix))
				{
					NextEnemyUnitToMove.ClearPath();
					NextEnemyUnitToMove.Movement = 0;
					NextEnemyUnitToMove.Flash = null;
					//_units.PlayerCheckForPossibleAttack(NextEnemyUnitToMove, _alliedEnemyMatrix);
					//FindEnemyUnitToAttackFrom();
				}
				else if (IsCellBlocked(NextEnemyUnitToMove, NextEnemyUnitMoveCoords.Col, NextEnemyUnitMoveCoords.Row))
				{
					NextEnemyUnitToMove.ClearPath();
					NextEnemyUnitToMove.Movement = 0;
				}
				else
				{
					NextEnemyUnitToMove.Col = NextEnemyUnitMoveCoords.Col;
					NextEnemyUnitToMove.Row = NextEnemyUnitMoveCoords.Row;

					NextEnemyUnitToMove.Movement -= _map[NextEnemyUnitToMove.Col, NextEnemyUnitToMove.Row].GroundUnitTerrainModifier;
					NextEnemyUnitMoveCoords = NextEnemyUnitToMove.GetNextWaypoint(NextEnemyUnitToMove.Col, NextEnemyUnitToMove.Row);

					WakeAdjacentAlliedUnits();

					_log.Debug($"HandleEnemyUnitMove() to:{NextEnemyUnitToMove.Coordinates} movement = {NextEnemyUnitToMove.Movement} [unit:{NextEnemyUnitToMove.UnitNumber}]");
				}

				_totalUnitsMovedOrAttacked++;
			}

			if (NextEnemyUnitToMove.IsAtDestination)
			{
				_log.Debug("HandleEnemyUnitMove() unit is at destination");
				_units.PlayerCheckForPossibleAttack(NextEnemyUnitToMove, _alliedEnemyMatrix);
				NextEnemyUnitToMove.Command = UNITCOMMAND.Destination;
				NextEnemyUnitToMove.Sleep = true; //TODO: if allied unit comes in range, then wake and attack
				EnemyUnitQueue.Dequeue();
				NextEnemyUnitToMove = null;
			}
			else if (NextEnemyUnitToMove.Movement < 0.1)
			{
				_log.Debug($"HandleEnemyUnitMove() unit is out of moves this turn [unit:{NextEnemyUnitToMove.UnitNumber}]");
				_units.PlayerCheckForPossibleAttack(NextEnemyUnitToMove, _alliedEnemyMatrix);
				EnemyUnitQueue.Dequeue();
				NextEnemyUnitToMove = null;
			}
			else
			{
				NextEnemyUnitToMove.Flash = new UnitFlash(2, 0.2f, new Color(100, 200, 100));
			}
		}

		private void WakeAdjacentAlliedUnits()
		{
			var adjacentUnits = _units.FindAllAdjacentUnits(NextEnemyUnitToMove.Col, NextEnemyUnitToMove.Row);
			foreach (var adjacentUnit in adjacentUnits)
			{
				if (_alliedEnemyMatrix.AreAllied(NextEnemyUnitToMove.Nationality, adjacentUnit.Nationality) == false)
				{
					adjacentUnit.Sleep = false;
				}
			}
		}

		//TODO: this is a duplicate method (also in GameBoard.cs)
		private bool IsCellBlocked(Unit unit, int endCol, int endRow)
		{
			// check to see if this unit cannot cross this terrain
			if (_map[endCol, endRow].Blocked(unit.UnitType))
			{
				return true;
			}

			// check to see if the destination cell is already occupied by another unit
			if (_units.FindUnit(endCol, endRow) != null)
			{
				return true;
			}

			return false;
		}

		private void HandleEnemyAttackingAllied(GameTime gameTime)
		{
			//_log.Debug("HandleEnemyAttackingAllied()");
			if (EnemyUnitAttacking != null && AlliedUnitUnderAttack != null)
			{
				// wake allied unit if being attacked
				AlliedUnitUnderAttack.Sleep = false;

				if (EnemyUnitAttacking.Flash == null)
				{
					_log.Debug("HandleEnemyAttackingAllied() set enemy unit attack flash");
					EnemyUnitAttacking.Flash = new UnitFlash(2, 0.2f, new Color(200, 100, 100));
					AlliedUnitUnderAttack.Flash = null;
				}

				if (EnemyUnitAttacking.Flash.Complete && AlliedUnitUnderAttack.Flash == null)
				{
					_log.Debug("HandleEnemyAttackingAllied() set allied unit under attack flash");
					AlliedUnitUnderAttack.Flash = new UnitFlash(2, 0.2f, new Color(200, 100, 100));
				}

				if (AlliedUnitUnderAttack.Flash != null && (EnemyUnitAttacking.Flash.Complete && AlliedUnitUnderAttack.Flash.Complete))
				{
					//_log.Debug("HandleEnemyAttackingAllied() compute attack");
					// compute attack
					_totalUnitsMovedOrAttacked++;
					EnemyUnitAttacking.UnitHasAttackedThisTurn = true;
					EnemyUnitAttacking.SkipTurn = true;
					_units.ComputeAttack(EnemyUnitAttacking, AlliedUnitUnderAttack);
					EnemyUnitAttacking.Flash = null;
					AlliedUnitUnderAttack.Flash = null;

					EnemyUnitAttacking = null;
					AlliedUnitUnderAttack = null;
					_units.UnselectUnits();
					
				}
			}
			else
			{
				// nobody left to attack
			}
		}

		public void FindEnemyUnitToAttackFrom()
		{
			//_log.Debug("FindEnemyUnitToAttackFrom()");
			for (var i = 0; i < _units.Count; i++)
			{
				if (_alliedEnemyMatrix.AreEnemies(_units[i].Nationality, NATIONALITY.USA) &&
					_units[i].UnitHasAttackedThisTurn == false)
				{
					var cells = _map.FindAdjacentCells(_units[i].Col, _units[i].Row, -1, _units[i].Range); //TODO: need to find all cells in firing range

					//TODO: update algorithm to attack the lowest defense unit first (to remove from board and prevent multiple counter attacks per turn).
					foreach (var cellCoordinates in cells)
					{
						var alliedUnit = _units.FindUnit(cellCoordinates.Col, cellCoordinates.Row);
						if (alliedUnit != null && _alliedEnemyMatrix.AreEnemies(alliedUnit.Nationality, _units[i].Nationality))
						{
							//_log.Debug("FindEnemyUnitToAttackFrom() allied unit under attack=" + alliedUnit.UnitNumber);
							AlliedUnitUnderAttack = alliedUnit;
							EnemyUnitAttacking = _units[i];
							return;
						}
					}
				}
			}

			AlliedUnitUnderAttack = null;
			EnemyUnitAttacking = null;
		}

		private Unit FindNextUnitToMove(GameTime gameTime)
		{
			//_log.Debug("FindNextUnitToMove()");

			var nextUnitToMove = FindEnemyUnitToMove();

			/*
			if (nextUnitToMove == null)
				//_log.Debug("nextUnitToMove=null");
			else
				//_log.Debug("nextUnitToMove=" + nextUnitToMove.UnitNumber);
			*/
			// find a unit to attack
			if (EnemyUnitAttacking == null)
			{
				FindEnemyUnitToAttackFrom();
				EnemyAttackFlash = 0;
			}

			HandleEnemyAttackingAllied(gameTime);

			return nextUnitToMove;
		}

		private Unit FindEnemyUnitToMove()
		{
			//_log.Debug("FindEnemyUnitToMove()");
			while (EnemyUnitQueue.Count > 0)
			{
				var unit = EnemyUnitQueue.Peek();
				//_log.Debug("FindEnemyUnitToMove() unit peek.  Unit #" + unit.UnitNumber);

				if (unit.Command == UNITCOMMAND.NearestEnemy)
				{
					//_log.Debug("FindEnemyUnitToMove() NearestEnemy command");
					//TODO: enemy unit is trying to zero in on the closest allied unit
					//TODO: need to account for visible mask

					// compute path to nearest enemy
					if (!_units.IsUnitWithinAttackRange(unit, _alliedEnemyMatrix))
					{
						var nearestEnemyUnit = _units.FindClosestUnit(unit.Col, unit.Row, NATIONALITY.USA);
						unit.DestCol = _units[nearestEnemyUnit].Col;
						unit.DestRow = _units[nearestEnemyUnit].Row;
						unit.ComputePath((TerrainMap) _map);

						NextEnemyUnitMoveCoords = unit.GetNextWaypoint(unit.Col, unit.Row);
					}
					else
					{
						_units.PlayerCheckForPossibleAttack(unit, _alliedEnemyMatrix);
						NextEnemyUnitToMove = null;
						unit.RedHilight = false;
						unit.IdleFlash = false;
					}
					return unit;
				}

				//_log.Debug("FindEnemyUnitToMove() not NearestEnemy command");
				var lCoordinates = unit.GetNextWaypoint(unit.Col, unit.Row);

				if (lCoordinates == null)
				{
					//_log.Debug("FindEnemyUnitToMove() coordinates=null, put unit to sleep");
					// use the destination set, if there is one
					//lCoordinates = FindEnemyUnitClosestDestination(unit);
					unit.Sleep = true;
				}

				// check to see if the map is occupied by another unit
				if (lCoordinates != null)
				{
					if (!_units.IsMapOccupied(lCoordinates.Col, lCoordinates.Row))
					{
						//_log.Debug("FindEnemyUnitToMove() map not occupied by another unit");

						NextEnemyUnitMoveCoords = new MapCoordinates(lCoordinates.Col, lCoordinates.Row);
						//_log.Debug("FindEnemyUnitToMove() NextEnemyUnitMoveCoords=" + NextEnemyUnitMoveCoords.Coordinates);

						return unit;
					}

					// can we attack the other unit?
					var otherUnit = _units.FindUnit(lCoordinates.Col, lCoordinates.Row);
					if (otherUnit != null)
					{
						//_log.Debug("FindEnemyUnitToMove() otherUnit=" + otherUnit.UnitNumber);

						// occupied by friendly unit
						if (otherUnit.Nationality == NATIONALITY.Germany)
						{
							//_log.Debug("FindEnemyUnitToMove() other unit is friendly");

							// stuff the unit back in the queue
							// make sure we don't get into an infinite loop with this algorithm.  Need to limit the number of times a unit goes into the queue.
							if (unit.UnitWait > 10)
							{
								unit.SkipTurn = true;
								EnemyUnitQueue.Dequeue();
							}
							else
							{
								EnemyUnitQueue.Dequeue();
								EnemyUnitQueue.Enqueue(unit);
							}

							unit.UnitWait++;
						}
						else
						{
							//_log.Debug("FindEnemyUnitToMove() other unit is enemy, attack");

							// attack allied unit
							EnemyUnitQueue.Dequeue();
						}
					}
				}
				else
				{
					//_log.Debug("FindEnemyUnitToMove() skip turn.");
					unit.SkipTurn = true;
					EnemyUnitQueue.Dequeue();
				}
			}

			return null;
		}

		private MapCoordinates FindEnemyUnitClosestDestination(Unit unit)
		{
			//_log.Debug("FindEnemyUnitClosestDestination()");

			var surroundingCells = _map.FindAdjacentCells(unit.Col, unit.Row, unit.UnitType, 1);
			double shortestDistance = 9999;
			int shortestDistanceItem = -1;

			// choose the next closest cell between the unit and dest. 
			for (int i = 0; i < surroundingCells.Count; i++)
			{
				if (!_units.IsMapOccupied(surroundingCells[i].Col, surroundingCells[i].Row))
				{
					double distance = BattleFieldOneCommonObjects.Distance(surroundingCells[i].Col, surroundingCells[i].Row, unit.Col,
						unit.Row);
					if (distance < shortestDistance)
					{
						shortestDistance = distance;
						shortestDistanceItem = i;
					}
				}
			}

			if (shortestDistanceItem > -1)
			{
				return surroundingCells[shortestDistanceItem];
			}

			return null;
		}

		public void SetEnemyStrategy(STRATEGY strategy)
		{
			//_log.Debug("SetEnemyStrategy()" + strategy);
			_currentStrategy = strategy;

			if (_map.TotalCities == 0)
			{
				return;
			}

			if (strategy == STRATEGY.Defend)
			{
				foreach (var unit in _units)
				{
					if (unit.Nationality == NATIONALITY.Germany)
					{
						unit.Command = UNITCOMMAND.Defend;
						unit.DestCol = unit.Col;
						unit.DestRow = unit.Row;
					}
				}
				return;
			}

			if (strategy == STRATEGY.CaptureAllCities)
			{
				CaptureAllCitiesStrategy();
			}

			if (strategy == STRATEGY.AttackAllUnits)
			{
				AttackAllUnitsStrategy();
			}
		}

		private void CaptureAllCitiesStrategy()
		{
			// split into groups of units and set each group of units to a different city
			var liTotalGermanUnits = _units.TotalGermanUnits;

			var liUnitsInGroup = liTotalGermanUnits / _map.TotalCities;

			// this means that we can't hold all 4 cities (because there are less units than cities), just try to block the allied units
			if (liUnitsInGroup < 1)
			{
				liUnitsInGroup = 1;
			}

			// now find the closest "liUnitsIngroup" number of german units to each city
			foreach (var mapCoords in _map.CityList)
			{
				var liTotalUnitsAssigned = 0;
				while (liTotalUnitsAssigned < liUnitsInGroup)
				{
					// find the next german unit not assigned
					var liNextClosestGermanUnit = _units.FindClosestUnassignedUnit(mapCoords.Col, mapCoords.Row, NATIONALITY.Germany);

					// must have run out of units (this can happen if the number of units down divide evenly)
					if (liNextClosestGermanUnit == -1)
					{
						break;
					}

					// if this one is the shortest distance, then assign it the destination
					_units[liNextClosestGermanUnit].Command = UNITCOMMAND.Destination;

					_units[liNextClosestGermanUnit].DestCol = mapCoords.Col;
					_units[liNextClosestGermanUnit].DestRow = mapCoords.Row;

					_units[liNextClosestGermanUnit].ComputePath((TerrainMap)_map);

					liTotalUnitsAssigned++;
				}
			}

			// any remaining units, choose closest cities
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.Germany)
				{
					if (unit.Command == UNITCOMMAND.None)
					{
						var closestCity = _map.FindClosestCity(unit.Col, unit.Row);
						if (closestCity != null)
						{
							unit.Command = UNITCOMMAND.Destination;

							unit.DestCol = closestCity.Col;
							unit.DestRow = closestCity.Row;

							unit.ComputePath((TerrainMap)_map);
						}
					}
				}
			}
		}

		private void AttackAllUnitsStrategy()
		{
			//TODO: leave one unit on a city.

			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.Germany)
				{
					unit.Command = UNITCOMMAND.NearestEnemy;

					if (!_units.IsUnitWithinAttackRange(unit, _alliedEnemyMatrix))
					{
						var nearestEnemyUnit = _units.FindClosestUnit(unit.Col, unit.Row, NATIONALITY.USA);
						unit.DestCol = _units[nearestEnemyUnit].Col;
						unit.DestRow = _units[nearestEnemyUnit].Row;
						unit.ComputePath((TerrainMap)_map);

						NextEnemyUnitMoveCoords = unit.GetNextWaypoint(unit.Col, unit.Row);
					}
					else
					{
						_units.PlayerCheckForPossibleAttack(unit, _alliedEnemyMatrix);
					}
				}
			}
		}

		public bool AreAllEnemyUnitsStuck()
		{
			//_log.Debug("AreAllEnemyUnitsStuck()");
			// are all units in sleep mode?
			//if (_units.Where(unit => unit.Nationality == NATIONALITY.Germany).All(unit => unit.Sleep || unit.SkipTurn)) return true;
			return false;
		}

	}
}
