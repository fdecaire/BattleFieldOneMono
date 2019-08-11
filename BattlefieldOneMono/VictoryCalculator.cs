using BattlefieldOneMono.Interfaces;
using NLog;

namespace BattlefieldOneMono
{
	public class VictoryCalculator : IVictoryCalculator
	{
		private int _totalAlliedCitiesToCapture;
		private int _totalGermanCitiesToCapture;
		private int _totalGermanCitiesToDefend;
		private int _totalAlliedCitiesToDefend;
		private readonly ITerrainMap _terrainMap;
		private readonly IUnitList _units;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

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

		public void GermanCitiesToDefend(int totalCities)
		{
			_totalGermanCitiesToDefend = totalCities;
		}

        public int TotalAlliedUnitsAlive
        {
            get
            {
                var total = 0;
                foreach (var unit in _units)
                {
                    if (unit.Nationality == NATIONALITY.USA)
                    {
                        total++;
                    }
                }

                return total;
            }
        }

        public int TotalGermanUnitsAlive
        {
            get
            {
                var total = 0;
                foreach (var unit in _units)
                {
                    if (unit.Nationality == NATIONALITY.Germany)
                    {
                        total++;
                    }
                }

                return total;
            }
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
                    _log.Debug("Axis Forces Failed to Defend " + _totalGermanCitiesToDefend + " Cit" + (_totalGermanCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!");
					return "Axis Forces Failed to Defend " + _totalGermanCitiesToDefend + " Cit" + (_totalGermanCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!";
				}
			}
			else
			{
				if (liTotal >= _totalGermanCitiesToCapture)
				{
					if (_terrainMap.CityList.Count == liTotal)
                    {
                        _log.Debug("Axis Forces Captured All Cities!");
                        return "Axis Forces Captured All Cities!";
					}

                    _log.Debug("Axis Forces Captured " + liTotal + " Cit" + (_totalGermanCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!");
                    return "Axis Forces Captured " + liTotal + " Cit" + (_totalGermanCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
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
                    _log.Debug("Allies Failed to Defend " + _totalAlliedCitiesToDefend + " Cit" + (_totalAlliedCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!");
                    return "Allies Failed to Defend " + _totalAlliedCitiesToDefend + " Cit" + (_totalAlliedCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!";
				}
			}
			else
			{
				if (liTotal >= _totalAlliedCitiesToCapture)
				{
					if (liTotal == _terrainMap.CityList.Count)
					{
                        _log.Debug("Allies Captured All Cities!");
                        return "Allies Captured All Cities!";
					}

                    _log.Debug("Allies Captured " + liTotal + " Citi" + (_totalAlliedCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!");
                    return "Allies Captured " + liTotal + " Citi" + (_totalAlliedCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
                }
			}

            // check to see if all german units destroyed
			if (TotalGermanUnitsAlive == 0)
			{
                _log.Debug("All Axis Units Destroyed!");
                return "All Axis Units Destroyed!";
			}

			// check to see if all allied units destroyed
			if (TotalAlliedUnitsAlive == 0)
			{
                _log.Debug("All Allied Units Destroyed!");
                return "All Allied Units Destroyed!";
			}

			return "";
		}
	}
}
