using System.ComponentModel;

namespace BattlefieldOneMono.Interfaces
{
	public interface IVictoryCalculator
	{
		void AlliesCaptureCitiesToWin(int totalCities);
		void GermanCaptureCitiesToWin(int totalCities);
		void AlliedCitiesToDefend(int totalCities);
		void GermanCitiesToDefend(int totalCities);
        int TotalAlliedUnitsAlive { get; }
        int TotalGermanUnitsAlive { get; }
		string Result();
	}
}
