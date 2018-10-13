using System.Collections.Generic;

namespace BattlefieldOneMono
{
	public class AStarNodeList
	{
		private readonly List<AStarNode> _items = new List<AStarNode>();

		public int Count => _items.Count;

		public void Push(AStarNode node)
		{
			_items.Add(node);
		}

		public AStarNode Pop()
		{
			if (_items.Count > 0)
			{
				var node = _items[_items.Count - 1];
				_items.RemoveAt(_items.Count - 1);
				return node;
			}
			return null;
		}

		public AStarNode FindSmallestNode()
		{
			// find the smallest node and remove from list, return node
			var smallestNumber = double.MaxValue;
			var smallestNodeNumber = -1;
			for (var i = 0; i < _items.Count; i++)
			{
				if (_items[i].F < smallestNumber)
				{
					smallestNumber = _items[i].F;
					smallestNodeNumber = i;
				}
			}

			if (smallestNodeNumber > -1)
			{
				var node = _items[smallestNodeNumber];
				_items.RemoveAt(smallestNodeNumber);
				return node;
			}

			return null;
		}

		public bool Contains(int x, int y)
		{
			return _items.FindIndex(t => t.X == x && t.Y == y) > -1;
		}

		public void UpdateNodeIfBetter(AStarNode node)
		{
			// if the node passed in has a better "G" rating, then replace the old node
			var index = _items.FindIndex(t => t.X == node.X && t.Y == node.Y);
			if (index > -1)
			{
				if (node.G < _items[index].G)
				{
					_items[index].G = node.G;
					_items[index].H = node.H;
					_items[index].Source.X = node.Source.X;
					_items[index].Source.Y = node.Source.Y;
				}
			}
		}

		public AStarNode GetNode(int x, int y)
		{
			var index = _items.FindIndex(t => t.X == x && t.Y == y);
			if (index > -1)
			{
				return _items[index];
			}
			return null;
		}

		public void Clear()
		{
			_items.Clear();
		}
	}
}
