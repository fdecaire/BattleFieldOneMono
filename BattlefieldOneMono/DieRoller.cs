using System;
using BattlefieldOneMono.Interfaces;

namespace BattlefieldOneMono
{
	public class DieRoller : IDieRoller
	{
		private static readonly Random RandomNumberGenerator = new Random(DateTime.Now.Millisecond);

		public int DieRoll()
		{
			return RandomNumberGenerator.Next() % 6 + 1;
		}
	}
}
