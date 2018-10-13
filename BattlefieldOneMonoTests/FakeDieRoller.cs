using System.Collections.Generic;
using BattlefieldOneMono;

namespace BattlefieldOneXNATests
{
	public class FakeDieRoller : IDieRoller
	{
		private int _nextDieRoll;
		private readonly List<int> _setDieRoll = new List<int>();
		public int SetDieRoll
		{
			get
			{
				int nextDieRoll = _setDieRoll[_nextDieRoll];
				_nextDieRoll++;
				if (_nextDieRoll >= _setDieRoll.Count)
				{
					_nextDieRoll = 0;
				}

				return nextDieRoll;
			}
			set
			{
				_setDieRoll.Add(value);
			}
		}

		public void ClearDieRoll()
		{
			_setDieRoll.Clear();
			_nextDieRoll = 0;
		}

		public int DieRoll()
		{
			return SetDieRoll;
		}
	}
}
