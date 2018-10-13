using System;
using System.IO;

namespace BattlefieldOneMono
{
	public class GameFileReader
	{
		private readonly ITerrainMap _terrainMap;
		private readonly IUnitList _units;
		private static Random _rnd = new Random();

		public GameFileReader(ITerrainMap terrainMap, IUnitList unitList)
		{
			_terrainMap = terrainMap;
			_units = unitList;
		}

		public void ReadGameFile(string fileName)
		{
			using (var file = new StreamReader(fileName))
			{
				string line;
				while ((line = file.ReadLine()) != null)
				{
					switch (line.ToLower())
					{
						case "map":
							DecodeMap(file);
							break;

						case "units":
							DecodeUnits(file);
							break;

						case "roads":
							DecodeRoads(file);
							break;
					}
				}
			}
		}

		private void DecodeRoads(StreamReader file)
		{
			var line = file.ReadLine();
			int totalRoads = int.Parse(line);

			for (int i = 0; i < totalRoads; i++)
			{
				line = file.ReadLine();
				string[] roadEnds = line.Split('-');
				string[] roadStart = roadEnds[0].Split(',');
				string[] roadEnd = roadEnds[1].Split(',');

				int startX = int.Parse(roadStart[0]);
				int startY = int.Parse(roadStart[1]);
				int endX = int.Parse(roadEnd[0]);
				int endY = int.Parse(roadEnd[1]);

				_terrainMap.PlotRoad(startX, startY, endX, endY);
			}
		}

		private void DecodeUnits(StreamReader file)
		{
			var line = file.ReadLine();
			int totalUnits = int.Parse(line);

			for (int i = 0; i < totalUnits; i++)
			{
				line = file.ReadLine();
				string[] rawUnitData = line.Split(',');

				//TODO: verify that there are 4 cells
				//16, -7, 0, G
				int x = int.Parse(rawUnitData[0]);
				int y = int.Parse(rawUnitData[1]);
				int type = int.Parse(rawUnitData[2]);

				AddUnit(x, y, type, rawUnitData[3].Trim().ToLower() == "a" ? NATIONALITY.USA : NATIONALITY.Germany);
			}
		}

		private void DecodeMap(StreamReader file)
		{
			var line = file.ReadLine();
			var temp = line.Split(',');

			// need to assert if not equal to two cells
			_terrainMap.Width = int.Parse(temp[0]);
			_terrainMap.Height = int.Parse(temp[1]);

			// read the map
			for (var y = 0; y < _terrainMap.Height; y++)
			{
				line = file.ReadLine();
				var tempRow = line.Split(',');

				for (var x = 0; x < tempRow.Length; x++)
				{
					var cube = HexGridMath.OffsetToCube(x, y);
					_terrainMap[x, y] = new TerrainCell(cube.X, cube.Y, cube.Z, TerrainDecode(tempRow[x].Trim()));
				}
			}
		}

		private int TerrainDecode(string rawMapCell)
		{
			int segment = 0;

			string terrainType = rawMapCell.Substring(0, 1);
			string terrain = rawMapCell.Substring(1, rawMapCell.Length - 1);
			int terrainNum = 0;

			if (terrain == "##")
			{
				terrainNum = _rnd.Next(0, 5);
			}
			else
			{
				terrainNum = int.Parse(terrain);
			}
			
			switch (terrainType.ToLower())
			{
				case "g":
					segment = 0;
					break;
				case "b":
					segment = 10;
					break;
				case "w":
					segment = 20;
					break;
				case "m":
					segment = 30;
					break;
				case "c":
					segment = 40;
					break;
			}

			return segment + terrainNum;
		}

		public void AddUnit(int Col, int Row, int unitType, NATIONALITY nationality)
		{
			_units.Add(Col, Row, unitType, nationality);

			if (nationality == NATIONALITY.USA)
			{
				var newUnit = _units.FindUnit(Col, Row);
				_terrainMap.UnmaskBoard(Col, Row, newUnit.ViewRange);
				_terrainMap.SetView(Col, Row, newUnit.ViewRange);
			}
		}
	}
}
