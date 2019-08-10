using BattlefieldOneMono.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattlefieldOneMono
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class GameMain : Game
	{
		GraphicsDeviceManager graphics;
		Offset _selectedHex;
		MouseState _mouseState;
		Vector2 _mouseDragStart = Vector2.Zero;
		bool _mouseButtonWasPressed;
		KeyboardState _oldKeyboardState;
		private IGameBoard _gameBoard;

		public GameMain(IGameBoard gameBoard)
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			graphics.PreferredBackBufferWidth = 1920;  // set this value to the desired width of your window
			graphics.PreferredBackBufferHeight = 1200;   // set this value to the desired height of your window
			graphics.ApplyChanges();
			_gameBoard = gameBoard;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			GameContent.Content = Content;
			GameContent.Sb = new SpriteBatch(GraphicsDevice);

			GraphicHelpers.Pixel = new Texture2D(GraphicsDevice, 1, 1);
			GraphicHelpers.Pixel.SetData(new[] { Color.Red });

			_gameBoard.CreateMap();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}

			// place the red dot at the mouse coordinates
			_mouseState = Mouse.GetState();
			var pos = new Vector2(_mouseState.X, _mouseState.Y);


			var newKeyboardState = Keyboard.GetState();  // get the newest state

			if (_oldKeyboardState.IsKeyUp(Keys.N) && newKeyboardState.IsKeyDown(Keys.N))
			{
				// next turn for this unit
				_gameBoard.NextUnit = true;
			}
			else if (_oldKeyboardState.IsKeyUp(Keys.S) && newKeyboardState.IsKeyDown(Keys.S))
			{
				// unit sleep
				_gameBoard.SleepUnit = true;
			}
			else if (_oldKeyboardState.IsKeyUp(Keys.D) && newKeyboardState.IsKeyDown(Keys.D))
			{
				// dump game data to a text file for analysis
				_gameBoard.DumpGameData = true;
			}
			else if (_oldKeyboardState.IsKeyUp(Keys.T) && newKeyboardState.IsKeyDown(Keys.T))
			{
				// end the current turn
				_gameBoard.NextTurn = true;
			}

			if (newKeyboardState.IsKeyDown(Keys.LeftShift))
			{
				// temporarily show everything
				_gameBoard.ShowAll = true;
			}
			else
			{
				_gameBoard.ShowAll = false;
			}

			_oldKeyboardState = newKeyboardState;  // set the new state as the old state for next time

			var temp = HexGridMath.PixelToHex(pos.X, pos.Y);
			var cube = HexGridMath.HexToCube(temp.Q, temp.R);
			_selectedHex = cube.CubeToOffset();

			_gameBoard.Update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);

			_gameBoard.Draw();

			var currentMouse = Mouse.GetState();
			var pos = new Vector2(currentMouse.X, currentMouse.Y);
			if (pos.X > 0 && pos.Y > 0)
			{
				GameContent.Sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

				// show the q,r coordinates
				string output = pos.X + "," + pos.Y + " cell:" + _selectedHex.Col + "," + _selectedHex.Row;
				GameContent.Sb.DrawString(GameContent.Arial10Font, output, new Vector2(2, 2), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
				GameContent.Sb.End();
			}

			if (_mouseState.LeftButton == ButtonState.Pressed)
			{
				_mouseButtonWasPressed = true;
				if (_mouseDragStart == Vector2.Zero)
				{
					// user just clicked
					_gameBoard.SelectUnit(_mouseState.X, _mouseState.Y);
					_mouseDragStart = new Vector2(_mouseState.X, _mouseState.Y);
				}
				else
				{
					// user is dragging
					_gameBoard.PlayerDrawRoute(_mouseState.X, _mouseState.Y);
				}
			}
			else if (_mouseButtonWasPressed)
			{
				_gameBoard.PlayerUnitMouseUp(_mouseState.X, _mouseState.Y);
				_mouseDragStart = Vector2.Zero;
				_mouseButtonWasPressed = false;
				_gameBoard.ShowUnitRangeMask = false;
			}

			var result = _gameBoard.CheckForEndOfGameCondition();
			if (result != "")
			{
				// Game ended
				Window.Title = result;
			}

			base.Draw(gameTime);
		}
	}
}
