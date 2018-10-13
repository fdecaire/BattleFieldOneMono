using System;
using BattlefieldOneMono.Constants;

namespace BattlefieldOneMono
{
	public class AlliedEnemyMatrix : IAlliedEnemyMatrix
	{
		private ALLIANCETYPE[,] AllianceResult = new ALLIANCETYPE[Enum.GetNames(typeof(NATIONALITY)).Length, Enum.GetNames(typeof(NATIONALITY)).Length];

		/// <summary>
		/// initialize
		/// </summary>
		/// <param name="initializeDefault">if true, then setup the default alliances according to the beginning of WWII</param>
		public AlliedEnemyMatrix(bool initializeDefault)
		{
			// initialize matrix with default values, these could change durning game or be altered by a load game
			for (int i = 0; i < Enum.GetNames(typeof(NATIONALITY)).Length; i++)
			{
				for (int j = 0; j < Enum.GetNames(typeof(NATIONALITY)).Length; j++)
				{
					AllianceResult[i, j] = ALLIANCETYPE.None;
				}
			}

			if (initializeDefault)
			{
				SetAlliance(NATIONALITY.USA, NATIONALITY.France);
				SetAlliance(NATIONALITY.USA, NATIONALITY.GreatBritian);
				SetAlliance(NATIONALITY.France, NATIONALITY.GreatBritian);
				SetAlliance(NATIONALITY.Germany, NATIONALITY.Japan);
				SetAlliance(NATIONALITY.Germany, NATIONALITY.Italy);
				SetAlliance(NATIONALITY.Japan, NATIONALITY.Italy);

				SetEnemies(NATIONALITY.USA, NATIONALITY.Germany);
				SetEnemies(NATIONALITY.USA, NATIONALITY.Japan);
				SetEnemies(NATIONALITY.GreatBritian, NATIONALITY.Germany);
				SetEnemies(NATIONALITY.GreatBritian, NATIONALITY.Japan);
				SetEnemies(NATIONALITY.France, NATIONALITY.Germany);
				SetEnemies(NATIONALITY.France, NATIONALITY.Japan);
			}
		}

		/// <summary>
		/// set alliance between two nationalities
		/// </summary>
		/// <param name="nationality1"></param>
		/// <param name="nationality2"></param>
		public void SetAlliance(NATIONALITY nationality1, NATIONALITY nationality2)
		{
			AllianceResult[(int)nationality1, (int)nationality2] = ALLIANCETYPE.Allied;
			AllianceResult[(int)nationality2, (int)nationality1] = ALLIANCETYPE.Allied;
		}

		public void SetEnemies(NATIONALITY nationality1, NATIONALITY nationality2)
		{
			AllianceResult[(int)nationality1, (int)nationality2] = ALLIANCETYPE.Enemy;
			AllianceResult[(int)nationality2, (int)nationality1] = ALLIANCETYPE.Enemy;
		}

		public bool AreAllied(NATIONALITY nationality1, NATIONALITY nationality2)
		{
			return AllianceResult[(int)nationality1, (int)nationality2] == ALLIANCETYPE.Allied;
		}

		public bool AreEnemies(NATIONALITY nationality1, NATIONALITY nationality2)
		{
			return AllianceResult[(int)nationality1, (int)nationality2] == ALLIANCETYPE.Enemy;
		}

		public bool AreAlliedOrEqual(NATIONALITY nationality1, NATIONALITY nationality2)
		{
			if (AllianceResult[(int)nationality1, (int)nationality2] == ALLIANCETYPE.Allied)
			{
				return true;
			}
			return nationality1 == nationality2;
		}
	}
}
