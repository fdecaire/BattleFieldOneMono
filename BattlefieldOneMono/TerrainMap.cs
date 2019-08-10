using System;
using System.Collections;
using System.Collections.Generic;
using BattlefieldOneMono.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	public class TerrainMap : ITerrainMap
	{
		public Hashtable Map = new Hashtable();
		public int Width { get; set; }
		public int Height { get; set; }
		private IShortestPath _shortestPath;
		public TerrainMap(IShortestPath shortestPath)
		{
			_shortestPath = shortestPath;
		}

		public long FindMapHash(int q, int r)
		{
			return q ^ (r + 0x9e3779b9 + (q << 6) + (q >> 2));
		}

		public long FindMapHash(int x, int y, int z)
		{
			var hex = HexGridMath.CubeToHex(x, y, z);
			return FindMapHash(hex.Q, hex.R);
		}

		public ICollection Keys => Map.Keys;

		public TerrainCell this[long hashnumber]
		{
			get
			{
				return (TerrainCell)Map[hashnumber];
			}
			set
			{
				Map[hashnumber] = value;
			}
		}

		public TerrainCell this[int col, int row]
		{
			get
			{
				var cube = HexGridMath.OffsetToCube(col, row);
				return (TerrainCell)Map[FindMapHash(cube.X, cube.Y, cube.Z)];
			}
			set
			{
				var cube = HexGridMath.OffsetToCube(col, row);
				var hash = FindMapHash(cube.X, cube.Y, cube.Z);

				if (Map[hash] != null)
				{
					Map.Remove(hash);
				}

				Map.Add(hash, value);
			}
		}

		/// <summary>
		/// Find all adjacent cells that are passible by the unitType provided
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="unitType"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		public List<MapCoordinates> FindAdjacentCells(int col, int row, int unitType, int range)
		{
			var list = new List<MapCoordinates>();

			// first convert to cube coords
			var cube = HexGridMath.OffsetToCube(col, row);

			for (int dx = -range; dx <= range; dx++)
			{
				var maxDx = Math.Max(-range, -dx - range);
				var minDx = Math.Min(range, -dx + range);
				for (int dy = maxDx; dy <= minDx; dy++)
				{
					var dz = -dx - dy;
					if (dx != 0 || dy != 0 || dz != 0) // don't include the square occupied by the unit
					{
						var terrain = (TerrainCell)Map[FindMapHash(cube.X + dx, cube.Y + dy, cube.Z + dz)];
						if (terrain != null)
						{
							if (unitType == -1 || (!terrain.Blocked(unitType))) // don't include any blocked terrain (mountains, oceans)
							{
								var offset = HexGridMath.CubeToOffset(cube.X + dx, cube.Y + dy, cube.Z + dz);
								list.Add(new MapCoordinates(offset.Col, offset.Row));
							}
						}
					}
				}
			}

			return list;
		}

		public int TotalCities
		{
			get
			{
				int total = 0;
				foreach (long key in Map.Keys)
				{
					if (((TerrainCell)Map[key]).IsCity)
					{
						total++;
					}
				}

				return total;
			}
		}

		private List<MapCoordinates> _cityList;
		public List<MapCoordinates> CityList
		{
			get
			{
				if (_cityList != null) return _cityList;

				var laCities = new List<MapCoordinates>();

				for (var y = 0; y < Height; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						var cube = HexGridMath.OffsetToCube(x, y);
						if (((TerrainCell)Map[FindMapHash(cube.X, cube.Y, cube.Z)]).IsCity)
						{
							laCities.Add(new MapCoordinates(x, y));
						}
					}
				}
				_cityList = laCities;
				return _cityList;
			}
		}

		public void ClearMask()
		{
			foreach (long key in Map.Keys)
			{
				((TerrainCell)Map[key]).Mask = false;
			}
		}

		/// <summary>
		/// create a blank map of grass
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void InitializeBoard(int width, int height)
		{
			Width = width;
			Height = height;

			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					var cube = HexGridMath.OffsetToCube(x, y);
					Map[FindMapHash(cube.X, cube.Y, cube.Z)] = new TerrainCell(cube.X, cube.Y, cube.Z, 0);
				}
			}
		}

		public void UnmaskBoard(int col, int row, int range)
		{
			var cube = HexGridMath.OffsetToCube(col, row);
			for (var dx = -range; dx <= range; dx++)
			{
				var maxDx = Math.Max(-range, -dx - range);
				var minDx = Math.Min(range, -dx + range);
				for (var dy = maxDx; dy <= minDx; dy++)
				{
					var dz = -dx - dy;
					var terrain = (TerrainCell)Map[FindMapHash(cube.X + dx, cube.Y + dy, cube.Z + dz)];
					if (terrain != null)
					{
						terrain.Mask = false;
					}
				}
			}
		}

		/// <summary>
		/// using offset coords
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="range"></param>
		public void SetView(int col, int row, int range)
		{
			var cube = HexGridMath.OffsetToCube(col, row);
			for (var dx = -range; dx <= range; dx++)
			{
				var maxDx = Math.Max(-range, -dx - range);
				var minDx = Math.Min(range, -dx + range);
				for (var dy = maxDx; dy <= minDx; dy++)
				{
					var dz = -dx - dy;
					var terrain = (TerrainCell)Map[FindMapHash(cube.X + dx, cube.Y + dy, cube.Z + dz)];
					if (terrain != null)
					{
						terrain.View = true;
					}
				}
			}
		}

		public MapCoordinates FindClosestCity(int col, int row)
		{
			double closestDistance = 9999;
			var index = -1;

			for (var i = 0; i < CityList.Count; i++)
			{
				var distance = BattleFieldOneCommonObjects.Distance(CityList[i].Col, CityList[i].Row, col, row);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					index = i;
				}
			}

			if (index > -1)
			{
				return new MapCoordinates(CityList[index].Col, CityList[index].Row);
			}

			return null;
		}

		public void PlotRoad(int startCol, int startRow, int destCol, int destRow)
		{
			// find shortest path
			_shortestPath.ComputeShortestPath(this, startCol, startRow, destCol, destRow, -1);

			// add the starting cell to the path
			_shortestPath.WayPoint.Insert(0, new Offset(startCol, startRow));

			TerrainCell prevTerrain = null;
			Cube prevTerrainCube = null;
			foreach (var wayPointOffset in _shortestPath.WayPoint)
			{
				var terrainCube = HexGridMath.OffsetToCube(wayPointOffset.Col, wayPointOffset.Row);
				var terrain = (TerrainCell)Map[FindMapHash(terrainCube.X, terrainCube.Y, terrainCube.Z)];

				// figure out which sides of the cell are touching
				if (prevTerrain != null)
				{
					if (prevTerrainCube.Y == terrainCube.Y) // lower left to upper right
					{
						if (terrainCube.X > prevTerrainCube.X)
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road3;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road6;
							}
						}
						else
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road6;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road3;
							}
						}
					}
					else if (prevTerrainCube.Z == terrainCube.Z) // upper left to lower right
					{
						if (terrainCube.Y > prevTerrainCube.Y)
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road5;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road2;
							}
						}
						else
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road2;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road5;
							}
						}
					}
					else if (prevTerrainCube.X == terrainCube.X) // vertical line
					{
						if (terrainCube.Y > prevTerrainCube.Y)
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road4;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road1;
							}
						}
						else
						{
							if (prevTerrain.BackgroundType != 40)
							{
								prevTerrain.Roads |= RoadType.Road1;
							}
							if (terrain.BackgroundType != 40)
							{
								terrain.Roads |= RoadType.Road4;
							}
						}
					}
					else
					{
						// should never get here
						System.Diagnostics.Debug.Assert(false);
					}
				}

				prevTerrain = terrain;
				prevTerrainCube = terrainCube;
			}
		}

		public List<Line> LineList = new List<Line>();

		private void AddToList(Line line)
		{
			foreach (var l in LineList)
			{
				if (Math.Abs(l.Start.X - line.Start.X) < 0.1 &&
				    Math.Abs(l.Start.Y - line.Start.Y) < 0.1 &&
				    Math.Abs(l.End.X - line.End.X) < 0.1 &&
				    Math.Abs(l.End.Y - line.End.Y) < 0.1)
				{
					return;
				}
				if (Math.Abs(l.End.X - line.Start.X) < 0.1 &&
				    Math.Abs(l.End.Y - line.Start.Y) < 0.1 &&
				    Math.Abs(l.Start.X - line.End.X) < 0.1 &&
				    Math.Abs(l.Start.Y - line.End.Y) < 0.1)
				{
					return;
				}
			}

			LineList.Add(line);
		}

		public void RecordLinesForOneHex(int x, int z)
		{
			var offsets = new[] { new Point(40, 0), new Point(20, 35), new Point(-20, 35), new Point(-40, 0), new Point(-20, -35), new Point(20, -35) };

			var center = new Point(60 * x + GameContent.XOffset, 35 * x + z * 70 + GameContent.YOffset);

			var start = new Point(center.X + 20, center.Y - 35);
			var end = new Point(0, 0);
			for (var i = 0; i < 6; i++)
			{
				end.X = center.X + offsets[i].X;
				end.Y = center.Y + offsets[i].Y;

				var line = new Line(start.X, start.Y, end.X, end.Y);
				AddToList(line);

				start.X = end.X;
				start.Y = end.Y;
			}
		}

		public void FormGrid(int cols, int rows)
		{
			for (var row = 0; row < rows; row++)
			{
				for (var col = 0; col < cols; col++)
				{
					var cube = OffsetToCube(col, row);

					RecordLinesForOneHex(cube.X, cube.Z);
				}
			}
		}

		public Cube OffsetToCube(int col, int row)
		{
			var x = col;
			var z = row - (col - (col & 1)) / 2;
			var y = -x - z;

			return new Cube(x, y, z);
		}

		public void Draw()
		{
			if (LineList.Count == 0)
			{
				FormGrid(Width, Height);
			}

			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			foreach (var line in LineList)
			{
				GameContent.Sb.DrawLine(line.Start, line.End, Color.Black, 2);
			}
			GameContent.Sb.End();
		}

		public void Dump()
		{
			for (var y = 0; y < Height; y++)
			{
				var rowOfData = "";
				for (var x = 0; x < Width; x++)
				{
					if (rowOfData != "")
					{
						rowOfData += ".";
					}
					rowOfData += this[x, y].BackgroundType.ToString("00");
				}
				//_log.Debug(rowOfData);
			}
		}

		public void HighlightRange(int col, int row, int range, Color color)
		{
			var cube = HexGridMath.OffsetToCube(col, row);

			GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			for (var dx = -range; dx <= range; dx++)
			{
				for (var dy = Math.Max(-range, -dx - range); dy <= Math.Min(range, -dx + range); dy++)
				{
					var dz = -dx - dy;
					var center = new Point(60 * (dx+cube.X) + GameContent.XOffset-40, 35 * (dx+cube.X) + (dz+cube.Z) * 70 + GameContent.YOffset-35);
					GameContent.Sb.Draw(GameContent.WhiteMask, new Vector2(center.X, center.Y), color);
				}
			}
			GameContent.Sb.End();
		}
	}
}
