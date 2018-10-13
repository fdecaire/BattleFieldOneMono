namespace BattlefieldOneMono
{
	public interface IVictoryCalculator
	{
		void AlliesCaptureCitiesToWin(int totalCities);
		void GermanCaptureCitiesToWin(int totalCities);
		void AlliedCitiesToDefend(int totalCities);
		void GermanCitiesToDefend(int totalcities);
		string Result();
	}
}
