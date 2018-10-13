namespace BattlefieldOneMono
{
	public interface IAlliedEnemyMatrix
	{
		void SetAlliance(NATIONALITY nationality1, NATIONALITY nationality2);
		void SetEnemies(NATIONALITY nationality1, NATIONALITY nationality2);
		bool AreAllied(NATIONALITY nationality1, NATIONALITY nationality2);
		bool AreEnemies(NATIONALITY nationality1, NATIONALITY nationality2);
		bool AreAlliedOrEqual(NATIONALITY nationality1, NATIONALITY nationality2);
	}
}
