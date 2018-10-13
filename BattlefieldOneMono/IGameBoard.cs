using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public interface IGameBoard
	{
		bool DumpGameData { get; set; }
		bool NextTurn { get; set; }
		bool NextUnit { get; set; }
		bool ShowAll { get; set; }
		bool SleepUnit { get; set; }
		bool ShowUnitRangeMask { get; set; }

		string CheckForEndOfGameCondition();
		void CreateMap();
		void Draw();
		void PlayerDrawRoute(int pixelX, int pixelY);
		void PlayerMoveUnit(Unit unit, int col, int row);
		void PlayerUnitMouseUp(int pixelX, int pixelY);
		void RecomputeView();
		void SelectUnit(int pixelX, int pixelY);
		void Update(GameTime gameTime);
	}
}