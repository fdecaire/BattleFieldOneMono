﻿using System.Collections.Generic;

namespace BattlefieldOneMono
{
	public interface IShortestPath
	{
		void Dump(TerrainMap terrainMap);
		MapCoordinates GetNextWaypoint(int X, int Y);
		List<Offset> WayPoint { get; set; }
		void ComputeShortestPath(ITerrainMap terrainMap, int startCol, int startRow, int endCol, int endRow, int unitType);
		void Clear();
	}
}