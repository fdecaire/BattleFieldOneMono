using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldOneMono
{
	public class DieRoller : IDieRoller
	{
		private static readonly Random _randomNumberGenerator = new Random(DateTime.Now.Millisecond);

		public int DieRoll()
		{
			return _randomNumberGenerator.Next() % 6 + 1;
		}
	}
}
