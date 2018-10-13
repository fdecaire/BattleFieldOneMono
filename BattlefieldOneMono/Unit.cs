using System;
using BattlefieldOneMono.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	public class Unit : IComparable<Unit>, IUnit
	{
		public int UnitNumber { get; }
		public int Col { get; set; }
		public int Row { get; set; }
		private int _unitType;

		public int UnitType
		{
			get { return _unitType; }
			set
			{
				_unitType = value;
				switch (_unitType)
				{
					case 0: // troop
						Defense = 2;
						Offense = 1;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 1: // tank
						Defense = 16;
						Offense = 6;
						Movement = 3;
						Range = 1;
						ViewRange = 1;
						break;
					case 2: // artillery
						Defense = 2;
						Offense = 14;
						Movement = 1;
						Range = 3;
						ViewRange = 2;
						break;
					case 3: // team
						Defense = 4;
						Offense = 2;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 4: // squad
						Defense = 8;
						Offense = 4;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 5: // platoon
						Defense = 16;
						Offense = 8;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 6: // company
						Defense = 32;
						Offense = 16;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 7: // battalion
						Defense = 64;
						Offense = 32;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 8: // regiment
						Defense = 128;
						Offense = 64;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 9: // brigade
						Defense = 256;
						Offense = 128;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;
					case 10: // division
						Defense = 512;
						Offense = 256;
						Movement = 1;
						Range = 1;
						ViewRange = 1;
						break;

				}
				_initialMovement = Movement;
			}
		}

		public int Defense { get; set; }
		public int Offense { get; private set; }
		private double _movement;
		public double Movement
		{
			get
			{
				return _movement;
			}
			set
			{
				_movement = value;
				if (_movement < 0)
				{
					_movement = 0;
				}
			}
		}
		public int Range { get; private set; }
		public int ViewRange { get; private set; }
		private double _initialMovement;
		public bool Selected { get; set; }
		public bool IdleFlash { get; set; }
		private static int _nextUnitNumber = 1;
		public NATIONALITY Nationality { get; set; }
		public UNITCOMMAND Command { get; set; } // used by enemy units only
		public int DestCol { get; set; } // used by enemy units only
		public int DestRow { get; set; } // used by enemy units only
		public bool UnitHasAttackedThisTurn { get; set; }
		private readonly IShortestPath _shortestPath;
		public int UnitWait { get; set; }
		public bool RedHilight { get; set; }
		public bool GreenHilight { get; set; }
		public bool Sleep { get; set; }
		public bool SkipTurn { get; set; }
		public UnitFlash Flash { get; set; }

		//private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public Unit(IShortestPath shortestPath)
		{
			UnitNumber = _nextUnitNumber;
			_shortestPath = shortestPath;
			Selected = false;
			UnitHasAttackedThisTurn = false;
			Sleep = false;
			_nextUnitNumber++;
		}

		public bool TurnComplete
		{
			get
			{
				if (Movement < 0.1 && UnitHasAttackedThisTurn || Sleep || SkipTurn)
				{
					return true;
				}
				return false;
			}
		}

		public Cube ToCube
		{
			get
			{
				var cube = HexGridMath.OffsetToCube(Col, Row);
				return new Cube(cube.X, cube.Y, cube.Z);
			}
		}

		public bool IsAtDestination => Col == DestCol && Row == DestRow;

		// reset for a new turn
		public void NewTurn()
		{
			Movement = _initialMovement;
			UnitWait = 0;
			Selected = false;
			UnitHasAttackedThisTurn = false;
			RedHilight = false;
			GreenHilight = false;
			IdleFlash = false;
			SkipTurn = false;
		}

		public void Draw()
		{
			var width = GameContent.UnitSprite[UnitType].Width;
			var height = GameContent.UnitSprite[UnitType].Height;

			var cube = HexGridMath.OffsetToCube(Col, Row);

			// center point
			var lnX = 60 * cube.X + GameContent.XOffset;
			var lnY = 35 * cube.X + cube.Z * 70 + GameContent.YOffset;

			Color nationalityColor;
			if (Nationality == NATIONALITY.USA)
			{
				if (Sleep)
				{
					nationalityColor = new Color(129, 245, 129);
				}
				else
				{
					nationalityColor = new Color(208, 254, 208);
				}
			}
			else
			{
				nationalityColor = new Color(221, 221, 221);
			}

			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			GameContent.Sb.Draw(GameContent.UnitSprite[UnitType], new Vector2(lnX - (int)(width / 2.0), lnY - (int)(height / 2.0)), nationalityColor);
			GameContent.Sb.End();

			if (IdleFlash)
			{
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.UnitSprite[UnitType], new Vector2(lnX - (int)(width / 2.0), lnY - (int)(height / 2.0)), new Color(50, 200, 50));
				GameContent.Sb.End();
			}

			/*
			if (RedHilight)
			{
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.UnitSprite[UnitType], new Vector2(lnX - (int)(width / 2.0), lnY - (int)(height / 2.0)), new Color(200, 100, 100));
				GameContent.Sb.End();
			}

			if (GreenHilight)
			{
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
				GameContent.Sb.Draw(GameContent.UnitSprite[UnitType], new Vector2(lnX - (int)(width / 2.0), lnY - (int)(height / 2.0)), new Color(100, 200, 100));
				GameContent.Sb.End();
			}
			*/
			if (Flash != null)
			{
				if (Flash.UnitHighlighted)
				{
					GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
					GameContent.Sb.Draw(GameContent.UnitSprite[UnitType], new Vector2(lnX - (int)(width / 2.0), lnY - (int)(height / 2.0)), Flash.HighlightColor);
					GameContent.Sb.End();
				}
			}

			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

			// offense (upper left)
			GameContent.Sb.DrawString(GameContent.Arial10Font, Offense.ToString(), new Vector2(lnX - 20, lnY - 23), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

			// defense (lower left)
			GameContent.Sb.DrawString(GameContent.Arial10Font, Defense.ToString(), new Vector2(lnX - 20, lnY + 8), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

			// range (upper right)
			GameContent.Sb.DrawString(GameContent.Arial10Font, Range.ToString(), new Vector2(lnX + 22 - (int)(GameContent.Arial10Font.MeasureString(Range.ToString()).X), lnY - 23), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

			// movement (lower right)
			GameContent.Sb.DrawString(GameContent.Arial10Font, Movement.ToString(), new Vector2(lnX + 22 - (int)(GameContent.Arial10Font.MeasureString(Movement.ToString()).X), lnY + 8), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

			// unit number (temporary - upper center)
			GameContent.Sb.DrawString(GameContent.Arial6Font, UnitNumber.ToString(), new Vector2(lnX - (int)(GameContent.Arial6Font.MeasureString(UnitNumber.ToString()).X / 2.0), lnY - 22), Color.Red, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

			GameContent.Sb.End();
		}

		public void DumpData()
		{
			/*
			log.Debug("Unit Number:" + UnitNumber);
			log.Debug("Col, Row" + Col + "," + Row);
			log.Debug("Nationality:" + Nationality);
			log.Debug("Unit Type:" + UnitType);
			log.Debug("Offense:" + Offense);
			log.Debug("Defense:" + Defense);
			log.Debug("Movement:" + Movement);
			log.Debug("Turn Complete:" + (TurnComplete ? "Yes" : "No"));
			log.Debug("Sleep:" + (Sleep ? "Yes" : "No") + " Skip Turn:" + (SkipTurn ? "Yes" : "No") + " Unit attacked this turn:" + (UnitHasAttackedThisTurn ? "Yes" : "No"));

			// dump the path set
			foreach (var waypoint in _shortestPath.WayPoint)
			{
				log.Debug($"Waypoint:{waypoint.Col},{waypoint.Row}");
			}
			*/
		}

		int IComparable<Unit>.CompareTo(Unit otherUnit)
		{
			return UnitNumber.CompareTo(otherUnit.UnitNumber);
		}

		public void FindShortestPath(ITerrainMap terrainMap, int unitCol, int unitRow, int endCol, int endRow, int unitUnitType)
		{
			_shortestPath.ComputeShortestPath(terrainMap, unitCol, unitRow, endCol, endRow, unitUnitType);
		}

		public void ComputePath(TerrainMap terrainMap)
		{
			_shortestPath.ComputeShortestPath(terrainMap, Col, Row, DestCol, DestRow, UnitType);
		}


		public MapCoordinates GetNextWaypoint(int unitCol, int unitRow)
		{
			return _shortestPath.GetNextWaypoint(unitCol, unitRow);
		}

		public void ClearPath()
		{
			_shortestPath.Clear();
		}

		public string Coordinates => Col + "," + Row;
	}
}
