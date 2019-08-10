namespace BattlefieldOneMono.Interfaces
{
	public interface ITerrainCell
	{
		int BackgroundType { get; set; }
		double GroundUnitTerrainModifier { get; }
		bool IsCity { get; }
		bool Mask { get; set; }
		RoadType Roads { get; set; }
		bool View { get; set; }

		bool Blocked(int unitType);
		void Draw(bool alwaysView = false);
		void DrawSample();
	}
}