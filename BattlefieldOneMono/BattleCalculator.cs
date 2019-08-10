
using BattlefieldOneMono.Constants;
using BattlefieldOneMono.Interfaces;

namespace BattlefieldOneMono
{
	public class BattleCalculator : IBattleCalculator
	{
		private IDieRoller _dieRoller;

		public BattleCalculator(IDieRoller dieRoller)
		{
			_dieRoller = dieRoller;
		}

		public BATTLERESULT Result(int offense)
		{
			int liDieRoll = _dieRoller.DieRoll();
			liDieRoll += _dieRoller.DieRoll();
			liDieRoll += _dieRoller.DieRoll();

			if (liDieRoll > 10 && offense > 12)
			{
				return BATTLERESULT.EnemyTripleDamaged;
			}

			if (liDieRoll > 8 && offense > 1)
			{
				return BATTLERESULT.EnemyDoubleDamaged;
			}

			if (liDieRoll > 6)
			{
				return BATTLERESULT.EnemyDamaged;
			}

			return BATTLERESULT.None;
		}
	}
}
