using Microsoft.Xna.Framework;

namespace BattlefieldOneMono
{
	public class UnitFlash
	{
		private float _timeSinceLastFlash;
		private int _unitFlashCount;
		public bool UnitHighlighted => _unitFlashCount % 2 == 1;
		public Color HighlightColor { get; } // new Color(200, 100, 100);
		public float FlashSpeed { get; } // 0.2f = quickflash
		public int TotalFlashes { get; }
		public bool Complete { get; private set; }

		public UnitFlash(int totalFlashes, float flashSpeed, Color flashColor)
		{
			TotalFlashes = totalFlashes;
			FlashSpeed = flashSpeed;
			HighlightColor = flashColor;
			Complete = false;
		}

		public void Update(GameTime gameTime)
		{
			if (Complete)
			{
				return;
			}

			_timeSinceLastFlash += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (_timeSinceLastFlash > FlashSpeed)
			{
				_unitFlashCount++;
				_timeSinceLastFlash = 0;

				if (TotalFlashes == -1)
				{
					// this is to prevent integer overflow for endless flash
					_unitFlashCount %= 2;
				}

				if (TotalFlashes > 0 && _unitFlashCount > TotalFlashes * 2)
				{
					_unitFlashCount = 0;
					Complete = true;
				}
			}
		}
	}
}
