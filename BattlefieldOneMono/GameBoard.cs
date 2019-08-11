using BattlefieldOneMono.Constants;
using BattlefieldOneMono.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	//TODO: make enemy use roads as part of their shortest path algorithm
	//BUG: enemy artillery doesn't fire from a distance
	//TODO: bridges, can be destroyed and fixed by engineers
	//TODO: barbed wire, can be laid by engineers can stop infantry but not tanks
	//TODO: tank traps, can be erected by engineers, can stop tanks not infantry
	//TODO: minefields, can be placed or cleared by engineers
	//TODO: machine gun nests and bunkers?
	//TODO: ships, landing craft?
	//TODO: weather conditions with effects on movement
	//TODO: need roll-over stats, instructions

	//TODO: if allied unit contacts enemy unit sitting on city, then move in with any units within 5 hex squares

	//http://aigamedev.com/open/tutorial/influence-map-mechanics/
	//http://www.cgf-ai.com/docs/straatman_remco_killzone_ai.pdf
	//http://aima.cs.berkeley.edu/
	//http://www.hevi.info/tag/artificial-intelligence-in-real-time-strategy-games/

	public class GameBoard : IGameBoard
	{
		private readonly ITerrainMap _terrainMap;
		private readonly IUnitList _units;
		private readonly bool _noMaskView = true; // this is used for testing purposes
		private readonly IEnemyPlan _enemyPlan;
		private readonly IVictoryCalculator _endOfGame;
		private readonly IAlliedEnemyMatrix _alliedEnemyMatrix;
		private readonly IShortestPath _shortestPath;

		public bool ShowUnitRangeMask { get; set; } = false;

		//TODO: refactor this block of vars
		private Unit _alliedUnitAttacking;
		private Unit _enemyUnitUnderAttack;
		private int _alliedAttackFlash;
		private Unit _nextAlliedUnitToMove;

		public bool NextUnit { get; set; }
		public bool SleepUnit { get; set; }
		public bool DumpGameData { get; set; }
		public bool ShowAll { get; set; }
		public bool NextTurn { get; set; }

		private float _timeSinceLastFlash;
		private int _unitFlash;
		private const int NumberOfFlashes = 2;

		public GameBoard(ITerrainMap terrainMap, IUnitList unitList, IShortestPath shortestPath, IAlliedEnemyMatrix alliedEnemyMatrix, 
			IVictoryCalculator victoryCalculator, IEnemyPlan enemyPlan)
		{
			_terrainMap = terrainMap;
			_units = unitList;
			_shortestPath = shortestPath;
			_alliedEnemyMatrix = alliedEnemyMatrix;
			_endOfGame = victoryCalculator;
			_enemyPlan = enemyPlan;
		}

		public void CreateMap()
		{
			var gameFileReader = new GameFileReader(_terrainMap,_units);
            gameFileReader.ReadGameFile(@"BattlefieldOneMono.GameMaps.Game04.map");
            RecomputeView();

			if (_noMaskView)
			{
				_terrainMap.ClearMask();
			}

			_units.CurrentTurn = NATIONALITY.USA;

			_enemyPlan.SetEnemyStrategy(STRATEGY.CaptureAllCities);

			_endOfGame.AlliesCaptureCitiesToWin(_terrainMap.CityList.Count);
			_endOfGame.GermanCaptureCitiesToWin(_terrainMap.CityList.Count);
		}

		public string CheckForEndOfGameCondition()
		{
			return _endOfGame.Result();
		}

		public void Update(GameTime gameTime)
		{
			_units.Update(gameTime);

			if (DumpGameData)
			{
				_units.DumpData();
				DumpGameData = false;
			}

			// TODO: move this section to its own object/method
			if (_units.CurrentTurn == NATIONALITY.USA)
			{
				HandleNextActiveUnit();

				HandleSleepingUnit();

				// make selected unit flash on/off
				CheckIdleUnit();

				HandleUnitMovement(gameTime);

				HandleAlliedAttackingEnemy(gameTime);

				if (_units.IsTurnComplete(NATIONALITY.USA) || NextTurn)
				{
					_units.ClearAllFlash();
					_units.IdleUnit = null;
					_units.SetTurn(NATIONALITY.Germany);
					_enemyPlan.StartEnemyTurn();
				}
			}

			_enemyPlan.HandleEnemy(gameTime, _noMaskView || ShowAll);

			NextUnit = false;
			SleepUnit = false;
			NextTurn = false;
		}

		private void ShowUnitMovementBoundries()
		{
			if (_units.IdleUnit == null || ShowUnitRangeMask == false)
			{
				return;
			}

			// get unit col,row
			var col = _units.IdleUnit.Col;
			var row = _units.IdleUnit.Row;

			// get unit movement limit
			var movement = _units.IdleUnit.Movement;

			// highlight all terrain cells around center of unit cell
			_terrainMap.HighlightRange(col, row, (int)movement, new Color(0, 30, 0, 70));
		}

		private void ShowUnitFireBoundries()
		{
			if (_units.IdleUnit == null || ShowUnitRangeMask == false)
			{
				return;
			}

			// get unit col,row
			var col = _units.IdleUnit.Col;
			var row = _units.IdleUnit.Row;

			// get unit attack range limit
			var movement = _units.IdleUnit.Range;

			// highlight all terrain cells around center of unit cell
			_terrainMap.HighlightRange(col, row, movement, new Color(30, 0, 0, 70));
		}

		private void HandleSleepingUnit()
		{
			if (SleepUnit)
			{
				_units.SetCurrentUnitSleep();
			}
		}

		private void HandleNextActiveUnit()
		{
			if (NextUnit)
			{
				_units.SetNextUnit();
			}
		}

		private void HandleUnitMovement(GameTime gameTime)
		{
			if (_nextAlliedUnitToMove != null)
			{
				_timeSinceLastFlash += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (_timeSinceLastFlash > 0.2f)
				{
					_unitFlash++;
					_timeSinceLastFlash = 0;

					if (_unitFlash > NumberOfFlashes * 2)
					{
						var lCoordinates = _nextAlliedUnitToMove.GetNextWaypoint(_nextAlliedUnitToMove.Col, _nextAlliedUnitToMove.Row);
						if (lCoordinates == null)
						{
							// we are at the destination already
							_nextAlliedUnitToMove = null;
							return;
						}

						if (IsCellBlocked(_nextAlliedUnitToMove, lCoordinates.Col, lCoordinates.Row))
						{
							_units.PlayerCheckForPossibleAttack(_nextAlliedUnitToMove, _alliedEnemyMatrix);

							_nextAlliedUnitToMove = null;
							if (_nextAlliedUnitToMove != null)
							{
								_nextAlliedUnitToMove.ClearPath(); //TODO: for now, we don't want _units to continue automatically the next turn.
							}
							return;
						}

						//_log.Debug($"HandleUnitMovement() Unit:{_nextAlliedUnitToMove.UnitNumber} before:" + _nextAlliedUnitToMove.Col + "," + _nextAlliedUnitToMove.Row);

						_nextAlliedUnitToMove.Movement -= _terrainMap[_nextAlliedUnitToMove.Col, _nextAlliedUnitToMove.Row].GroundUnitTerrainModifier;

						// move to the next dest
						PlayerMoveUnit(_nextAlliedUnitToMove, lCoordinates.Col, lCoordinates.Row);

						//_log.Debug($"HandleUnitMovement() After:" + _nextAlliedUnitToMove.Col + "," + _nextAlliedUnitToMove.Row);

						// ran out of moves this turn.  Stop.
						if (_nextAlliedUnitToMove.Movement < 0.1)
						{
							_units.PlayerCheckForPossibleAttack(_nextAlliedUnitToMove, _alliedEnemyMatrix);

							_nextAlliedUnitToMove = null;
							if (_nextAlliedUnitToMove != null)
							{
								_nextAlliedUnitToMove.ClearPath(); //TODO: for now, we don't want _units to continue automatically the next turn.
							}
						}
					}
				}
			}
		}

		private void HandleAlliedAttackingEnemy(GameTime gameTime)
		{
			if (_alliedUnitAttacking != null && _enemyUnitUnderAttack != null)
			{
				if (_alliedAttackFlash < 3)
				{
					switch (_alliedAttackFlash)
					{
						case 0:
							// turn allied unit red
							_alliedAttackFlash++;
							_timeSinceLastFlash = 0;
							break;
						case 1:
							_timeSinceLastFlash += (float)gameTime.ElapsedGameTime.TotalSeconds;
							if (_timeSinceLastFlash > 0.2f)
							{
								// turn enemy unit red
								_alliedAttackFlash++;
								_timeSinceLastFlash = 0;
							}
							break;
						case 2:
							_timeSinceLastFlash += (float)gameTime.ElapsedGameTime.TotalSeconds;
							if (_timeSinceLastFlash > 0.2f)
							{
								_alliedAttackFlash++;
							}
							break;
					}
				}
				else
				{
					// compute attack
					//_log.Debug($"HandleUnitMovement() allied ({_alliedUnitAttacking.UnitNumber}) attacking enemy:{_enemyUnitUnderAttack.UnitNumber}");

					_alliedUnitAttacking.UnitHasAttackedThisTurn = true;
					_units.ComputeAttack(_alliedUnitAttacking, _enemyUnitUnderAttack);

					_alliedUnitAttacking = null;
					_enemyUnitUnderAttack = null;
					_alliedAttackFlash = 0;
					_timeSinceLastFlash = 0;
					_units.UnselectUnits();
				}
			}
		}

		private void CheckIdleUnit()
		{
			// if the current unit's turn is complete, then move to the next unit
			if (_units.IdleUnit == null || _units.IdleUnit.TurnComplete)
			{
				_units.IdleUnit = _units.FindNextUnit(_units.IdleUnit, NATIONALITY.USA);
				if (_units.IdleUnit != null)
				{
					_units.IdleUnit.Flash = new UnitFlash(-1, 0.6f, new Color(100, 200, 100));
				}
			}
		}

		public void PlayerDrawRoute(int pixelX, int pixelY)
		{
			var startUnit = _units.FindSelectedUnit();

			if (startUnit != null)
			{
				var cube = HexGridMath.HexToCube(HexGridMath.PixelToHex(pixelX, pixelY));
				var destination = HexGridMath.CubeToOffset(cube.X, cube.Y, cube.Z);

				if (startUnit.Col != destination.Col || startUnit.Row != destination.Row)
				{
					_shortestPath.ComputeShortestPath((TerrainMap)_terrainMap, startUnit.Col, startUnit.Row, destination.Col, destination.Row, startUnit.UnitType);

					if (_shortestPath.WayPoint.Count > 0)
					{
						_shortestPath.WayPoint.Insert(0, new Offset(startUnit.Col, startUnit.Row));
					}

					for (int i = 1; i < _shortestPath.WayPoint.Count; i++)
					{
						GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
						GraphicHelpers.DrawLine(_shortestPath.WayPoint[i - 1].ToPixel, _shortestPath.WayPoint[i].ToPixel, Color.Red, 6);
						GameContent.Sb.End();
					}
				}
			}
		}

		public void PlayerMoveUnit(Unit unit, int col, int row)
		{
			if (unit.Nationality == NATIONALITY.USA)
			{
				// make sure this doesn't go off the map
				if (_terrainMap[col, row] != null)
				{
					unit.Col = col;
					unit.Row = row;

					_terrainMap.UnmaskBoard(unit.Col, unit.Row, unit.ViewRange);
					RecomputeView();

					if (unit.Movement < 0.1)
					{
						_units.PlayerCheckForPossibleAttack(unit, _alliedEnemyMatrix);
					}
				}
			}
		}

		public void PlayerUnitMouseUp(int pixelX, int pixelY)
		{
			var cube = HexGridMath.HexToCube(HexGridMath.PixelToHex(pixelX, pixelY));
			var offset = HexGridMath.CubeToOffset(cube.X, cube.Y, cube.Z);

			var enemyUnit = _units.FindUnit(offset.Col, offset.Row);
			if (enemyUnit != null && _alliedEnemyMatrix.AreEnemies(NATIONALITY.USA, enemyUnit.Nationality))
			{
				AttackUnit(enemyUnit);
				return;
			}

			//TODO: need to tweak algorithm so an allied unit cannot slide past an enemy unit.  Can only back away to a square that has no enemy _units in any surrounding hex.

			// user just dropped the unit
			var unit = _units.FindSelectedUnit();

			if (unit != null)
			{
				if (unit.Movement < 0.1)
				{
					return;
				}

				_units.IdleUnit = unit;

				//check to see if this is a new hex location
				if (unit.Col != offset.Col || unit.Row != offset.Row)
				{
					//TODO: need to account for coordinates that are off the map (negatives, etc.)
					int endCol = offset.Col;
					int endRow = offset.Row;

					// find the path
					unit.FindShortestPath(_terrainMap, unit.Col, unit.Row, endCol, endRow, unit.UnitType);

					_nextAlliedUnitToMove = unit;
				}
				else
				{
					// user clicked on unit, wake unit and make this unit the active unit
					if (!unit.TurnComplete)
					{
						unit.Sleep = false;
					}
				}

				return;
			}

			_units.UnselectUnits();
		}

		private bool IsCellBlocked(Unit unit, int endCol, int endRow)
		{
			// check to see if this unit cannot cross this terrain
			if (_terrainMap[endCol, endRow].Blocked(unit.UnitType))
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

		private void AttackUnit(Unit enemyUnit)
		{
			_alliedUnitAttacking = _units.FindSelectedUnit();

			if (_alliedUnitAttacking == null)
			{
				return;
			}

			if (_alliedUnitAttacking.UnitHasAttackedThisTurn)
			{
				// this unit has already attacked.
				//TODO: need to change this if _units are able to attack more than once per turn
				_alliedUnitAttacking = null;
				return;
			}

			// if the enemy unit is not in range, then we can't attack
			var distance = Cube.Distance(_alliedUnitAttacking.ToCube, enemyUnit.ToCube);
			if (distance > _alliedUnitAttacking.Range)
			{
				return;
			}

			_enemyUnitUnderAttack = enemyUnit;
			_alliedAttackFlash = 0;
		}

		public void SelectUnit(int pixelX, int pixelY)
		{
			var cube = HexGridMath.HexToCube(HexGridMath.PixelToHex(pixelX, pixelY));
			var selected = HexGridMath.CubeToOffset(cube.X, cube.Y, cube.Z);

			var unit = _units.FindUnit(selected.Col, selected.Row);

			if (unit != null && _alliedEnemyMatrix.AreAlliedOrEqual(unit.Nationality, NATIONALITY.USA))
			{
				if (unit.TurnComplete && unit.Sleep == false)
				{
					unit.Selected = false;
					return;
				}

				// unselect previously selected unit
				foreach (var otherUnit in _units)
				{
					if (otherUnit != unit && otherUnit.Selected)
					{
						otherUnit.Selected = false;
					}
				}

				unit.Selected = true;
				unit.Sleep = false;
				_units.IdleUnit = unit;
				unit.Flash = new UnitFlash(-1, 0.6f, new Color(100, 200, 100));
				ShowUnitRangeMask = true;
			}
		}

		/// <summary>
		/// recompute the views of the entire board
		/// </summary>
		public void RecomputeView()
		{
			// first, reset all the views
			foreach (long key in _terrainMap.Keys)
			{
				_terrainMap[key].View = false;
			}

			// uncover only the allied _units
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.USA)
				{
					_terrainMap.SetView(unit.Col, unit.Row, unit.ViewRange);
				}
			}
		}



		public void Draw()
		{
			// draw terrain
			foreach (long key in _terrainMap.Keys)
			{
				_terrainMap[key].Draw(_noMaskView || ShowAll);
			}

			ShowUnitFireBoundries();
			ShowUnitMovementBoundries();

			// render the outlines
			_terrainMap.Draw();

			// draw unit list
			foreach (var unit in _units)
			{
				var terrainCell = _terrainMap[unit.Col, unit.Row];
				if (!terrainCell.Mask || _noMaskView || ShowAll)
				{
					if (terrainCell.View || _noMaskView || ShowAll)
					{
						unit.Draw();
					}
				}
			}

            foreach (var unit in _units)
            {
                unit.DrawPath();
            }
        }
	}
}
