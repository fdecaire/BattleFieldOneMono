﻿using BattlefieldOneMono.Constants;

namespace BattlefieldOneMono
{
	public interface IUnit
	{
		int UnitNumber { get; }
		int Col { get; set; }
		int Row { get; set; }
		int UnitType { get; set; }
		int Defense { get; set; }
		int Offense { get; }
		double Movement { get; set; }
		int Range { get; }
		int ViewRange { get; }
		bool Selected { get; set; }
		bool IdleFlash { get; set; }
		NATIONALITY Nationality { get; set; }
		UNITCOMMAND Command { get; set; } // used by enemy units only
		int DestCol { get; set; } // used by enemy units only
		int DestRow { get; set; } // used by enemy units only
		bool UnitHasAttackedThisTurn { get; set; }
		int UnitWait { get; set; }
		bool RedHilight { get; set; }
		bool Sleep { get; set; }
		bool SkipTurn { get; set; }
		bool TurnComplete { get; }
		void ComputePath(TerrainMap terrainMap);
		Cube ToCube { get; }
		bool IsAtDestination { get; }
		UnitFlash Flash { get; set; }
		void NewTurn();
		void Draw();
		void DumpData();
		MapCoordinates GetNextWaypoint(int unitCol, int unitRow);
	}
}
