using System.Collections.Generic;

namespace BattlefieldOneMono
{
	public class ShortestPath : IShortestPath
	{
		private AStarNodeList _openList;
		private AStarNodeList _closedList;
		public List<Offset> WayPoint { get; set; }  // final list of waypoints
		private int _startCol;
		private int _startRow;
		private int _endCol;
		private int _endRow;
		private int _unitType;
		private int _iterations; // if we go through too many iterations, then assume no path possible
		//private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private ITerrainMap _terrainMap;

		public void ComputeShortestPath(ITerrainMap terrainMap, int startCol, int startRow, int endCol, int endRow,
			int unitType)
		{
			_endCol = endCol;
			_endRow = endRow;
			_startCol = startCol;
			_startRow = startRow;
			_unitType = unitType;
			_terrainMap = terrainMap;
			_iterations = 0;

			_openList = new AStarNodeList();
			_closedList = new AStarNodeList();
			WayPoint = new List<Offset>();

			// push the starting cell
			var currentNode = new AStarNode(startCol, startRow, startCol, startRow, _endCol, _endRow, 0);
			_openList.Push(currentNode);

			FindPath();
		}

		private void FindPath()
		{
			_iterations++;

			if (WayPoint.Count > 0)
			{
				//log.Debug("WayPoint.Count > 0");
				return;
			}

			if (_iterations > 500)
			{
				//log.Debug("Iterations > 50");
				return;
			}

			// pop the top item off the openlist and find surrounding cells
			var node = _openList.Pop();

			//log.DebugFormat("FindPath({0},{1})", node.X, node.Y);

			_closedList.Push(node);

			var surroundingCells = _terrainMap.FindAdjacentCells(node.X, node.Y, _unitType, 1);

			foreach (var mapCoordinates in surroundingCells)
			{
				// skip any nodes that are already in the closed list
				if (!_closedList.Contains(mapCoordinates.Col, mapCoordinates.Row))
				{
					var distance = node.G;
					if (_terrainMap[mapCoordinates.Col, mapCoordinates.Row].Roads > 0)
					{
						distance += 0.5;
					}
					else
					{
						distance += 1;
					}

					if (_openList.Contains(mapCoordinates.Col, mapCoordinates.Row))
					{
						// check to see if this path is shorter than the one on the open list, if so, then update it, otherwise skip
						var tempNode = new AStarNode(node.X, node.Y, mapCoordinates.Col, mapCoordinates.Row, _endCol, _endRow, distance);

						_openList.UpdateNodeIfBetter(tempNode);
					}
					else
					{
						_openList.Push(new AStarNode(node.X, node.Y, mapCoordinates.Col, mapCoordinates.Row, _endCol, _endRow, distance));
					}
				}
			}

			var smallestNode = _openList.FindSmallestNode();

			if (smallestNode == null)
			{
				//log.Debug("smallestNode == null");
				return;
			}

			// check if this is the destination node
			if (smallestNode.X == _endCol && smallestNode.Y == _endRow)
			{
				// consolidate the actual path into the WayPoint list
				WayPoint.Add(new Offset(_endCol, _endRow));

				// walk back to the starting point
				var tempNode = _closedList.GetNode(smallestNode.Source.X, smallestNode.Source.Y);
				while (tempNode.X != _startCol || tempNode.Y != _startRow)
				{
					WayPoint.Insert(0, new Offset(tempNode.X, tempNode.Y));
					tempNode = _closedList.GetNode(tempNode.Source.X, tempNode.Source.Y);
				}

				// clear the open and closed lists
				_openList.Clear();
				_closedList.Clear();
				//log.Debug("success");
				return;
			}

			_openList.Push(smallestNode);
			FindPath();
		}

		public MapCoordinates GetNextWaypoint(int x, int y)
		{
			if (WayPoint.Count > 0)
			{
				// if there are waypoints set, then use those first
				var point = WayPoint[0];
				if (x == point.Col && y == point.Row)
				{
					WayPoint.RemoveAt(0);

					if (WayPoint.Count > 0)
					{
						point = WayPoint[0];
						var mapCoordinates = new MapCoordinates(point.Col, point.Row);
						return mapCoordinates;
					}
					return null;
				}
				else
				{
					point = WayPoint[0];
					var mapCoordinates = new MapCoordinates(point.Col, point.Row);
					return mapCoordinates;
				}
			}

			return null;
		}

		private bool WaypointContains(int col, int row)
		{
			foreach (var point in WayPoint)
			{
				if (point.Col == col && point.Row == row)
				{
					return true;
				}
			}

			return false;
		}

		// dump the shortest path to the log
		public void Dump(TerrainMap terrainMap)
		{
			for (var y = 0; y < terrainMap.Height; y++)
			{
				var line = "";
				for (var x = 0; x < terrainMap.Width; x++)
				{
					if (WaypointContains(x, y))
					{
						line += "*";
					}
					else
					{
						line += ".";
					}
				}
				//_log.Debug(line);
			}
		}

		public void Clear()
		{
			WayPoint = null;
		}
	}
}
