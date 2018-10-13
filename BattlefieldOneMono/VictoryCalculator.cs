﻿namespace BattlefieldOneMono
{
	public class VictoryCalculator : IVictoryCalculator
	{
		private int _totalAlliedCitiesToCapture = 0;
		private int _totalGermanCitiesToCapture = 0;
		private int _totalAlliedUnitsAlive = 0;
		private int _totalGermanUnitsAlive = 0;
		private int _totalGermanCitiesToDefend = 0;
		private int _totalAlliedCitiesToDefend = 0;
		private readonly ITerrainMap _terrainMap;
		private readonly IUnitList _units;

		public VictoryCalculator(ITerrainMap terrainMap, IUnitList units)
		{
			_terrainMap = terrainMap;
			_units = units;
		}
		public void AlliesCaptureCitiesToWin(int totalCities)
		{
			_totalAlliedCitiesToCapture = totalCities;
		}

		public void GermanCaptureCitiesToWin(int totalCities)
		{
			_totalGermanCitiesToCapture = totalCities;
		}

		public void AlliedCitiesToDefend(int totalCities)
		{
			_totalAlliedCitiesToDefend = totalCities;
		}

		public void GermanCitiesToDefend(int totalcities)
		{
			_totalGermanCitiesToDefend = totalcities;
		}

		public string Result()
		{
			// check to see if german units occupy all cities
			var liTotal = 0;
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.Germany || unit.Nationality == NATIONALITY.Japan || unit.Nationality == NATIONALITY.Italy)
				{
					if (_terrainMap[unit.Col, unit.Row].IsCity)
					{
						liTotal++;
					}
				}
			}

			if (_totalGermanCitiesToDefend > 0)
			{
				if (liTotal < _totalGermanCitiesToDefend)
				{
					return "Axis Forces Failed to Defend " + _totalGermanCitiesToDefend + " Cit" + (_totalGermanCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!";
				}
			}
			else
			{
				if (liTotal >= _totalGermanCitiesToCapture)
				{
					if (_terrainMap.CityList.Count == liTotal)
					{
						return "Axis Forces Captured All Cities!";
					}
					else
					{
						return "Axis Forces Captured " + liTotal + " Cit" + (_totalGermanCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
					}
				}
			}

			// check to see if allied units occupy all cities
			liTotal = 0;
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.USA || unit.Nationality == NATIONALITY.France || unit.Nationality == NATIONALITY.GreatBritian)
				{
					if (_terrainMap[unit.Col, unit.Row].IsCity)
					{
						liTotal++;
					}
				}
			}

			if (_totalAlliedCitiesToDefend > 0)
			{
				if (liTotal < _totalAlliedCitiesToDefend)
				{
					return "Allies Failed to Defend " + _totalAlliedCitiesToDefend + " Cit" + (_totalAlliedCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!";
				}
			}
			else
			{
				if (liTotal >= _totalAlliedCitiesToCapture)
				{
					if (liTotal == _terrainMap.CityList.Count)
					{
						return "Allies Captured All Cities!";
					}
					else
					{
						return "Allies Captured " + liTotal + " Citi" + (_totalAlliedCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
					}
				}
			}

			// check to see if all german units destroyed
			liTotal = 0;
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.Germany)
				{
					liTotal++;
				}
			}

			if (liTotal == 0)
			{
				return "All Axis Units Destroyed!";
			}

			// check to see if all allied units destroyed
			liTotal = 0;
			foreach (var unit in _units)
			{
				if (unit.Nationality == NATIONALITY.USA)
				{
					liTotal++;
				}
			}

			if (liTotal == 0)
			{
				return "All Allied Units Destroyed!";
			}

			return "";
		}
	}
}
