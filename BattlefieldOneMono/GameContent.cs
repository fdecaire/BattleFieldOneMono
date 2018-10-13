using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BattlefieldOneMono
{
	public class GameContent
	{
		public static ContentManager Content { get; set; }
		public static SpriteBatch Sb { get; set; }

		private static GameContent _instance;
		public static GameContent Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameContent
					{
						_redBox = Content.Load<Texture2D>("red_box"),
						_mask = Content.Load<Texture2D>("mask"),
						_greenMask = Content.Load<Texture2D>("greenmask"),
						_redMask = Content.Load<Texture2D>("redmask"),
						_whiteMask = Content.Load<Texture2D>("whitemask"),
						_arial10Font = Content.Load<SpriteFont>("Arial10"),
						_arial6Font = Content.Load<SpriteFont>("Arial6"),
						_terrainSprite = new List<Texture2D>()
					};

					LoadBackgroundTextures("grass", 5);        // 00
					LoadBackgroundTextures("beach", 3);        // 10
					LoadBackgroundTextures("ocean", 5);        // 20
					LoadBackgroundTextures("mountains_", 1);    // 30
					LoadBackgroundTextures("city", 1);         // 40
					LoadBackgroundTextures("forest_", 2);       // 50

					Texture2D texture;

					_instance._unitSprite = new List<Texture2D>();
					texture = Content.Load<Texture2D>("troop");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("armor");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("artillery");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("team");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("squad");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("platoon");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("company");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("battalion");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("regiment");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("brigade");
					_instance._unitSprite.Add(texture);
					texture = Content.Load<Texture2D>("division");
					_instance._unitSprite.Add(texture);

					LoadRoads();
				}

				return _instance;
			}
		}

		private static void LoadRoads()
		{
			_instance._roadSprite = new List<Texture2D>();
			for (var i = 1; i <= 6; i++)
			{
				var texture = Content.Load<Texture2D>("road" + i.ToString("00"));
				_instance._roadSprite.Add(texture);
			}
		}

		private static void LoadBackgroundTextures(string name, int total)
		{
			Texture2D texture;
			for (var i = 1; i < total + 1; i++)
			{
				texture = Content.Load<Texture2D>(name + i.ToString("00"));
				_instance._terrainSprite.Add(texture);
			}

			// make sure there are 10 textures for each type of background
			texture = Content.Load<Texture2D>("gray_hex");
			for (var i = total; i < 10; i++)
			{
				_instance._terrainSprite.Add(texture);
			}
		}

		private Texture2D _whiteMask;
		public static Texture2D WhiteMask => Instance._whiteMask;

		private Texture2D _redMask;
		public static Texture2D RedMask => Instance._redMask;

		private Texture2D _greenMask;
		public static Texture2D GreenMask => Instance._greenMask;

		private Texture2D _mask;
		public static Texture2D Mask => Instance._mask;

		private Texture2D _redBox;
		public static Texture2D RedBox => Instance._redBox;

		private SpriteFont _arial10Font;
		public static SpriteFont Arial10Font => Instance._arial10Font;

		private SpriteFont _arial6Font;
		public static SpriteFont Arial6Font => Instance._arial6Font;

		private List<Texture2D> _terrainSprite;
		public static List<Texture2D> TerrainSprite => Instance._terrainSprite;

		private List<Texture2D> _roadSprite;
		public static List<Texture2D> RoadSprite => Instance._roadSprite;


		private List<Texture2D> _unitSprite;
		public static List<Texture2D> UnitSprite => Instance._unitSprite;

		public static int XOffset => 80;

		public static int YOffset => 60;
	}
}
