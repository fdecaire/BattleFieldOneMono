using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BattlefieldOneMono.Interfaces
{
	public interface ITerrainMap
	{
		TerrainCell this[long hashnumber] { get; set; }
		TerrainCell this[int col, int row] { get; set; }

		List<MapCoordinates> CityList { get; }
		int Height { get; set; }
		ICollection Keys { get; }
		int TotalCities { get; }
		int Width { get; set; }

		void ClearMask();
		List<MapCoordinates> FindAdjacentCells(int col, int row, int unitType, int range);
		MapCoordinates FindClosestCity(int col, int row);
		long FindMapHash(int q, int r);
		long FindMapHash(int x, int y, int z);
		void InitializeBoard(int width, int height);
		void PlotRoad(int startCol, int startRow, int destCol, int destRow);
		void SetView(int col, int row, int range);
		void UnmaskBoard(int col, int row, int range);
		void Draw();
		void Dump();
		void HighlightRange(int col, int row, int range, Color color);
	}
}