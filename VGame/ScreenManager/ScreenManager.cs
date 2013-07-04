using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VGame {
	public class ScreenManager : DrawableGameComponent {
		List<GameScreen> screens = new List<GameScreen>();
		List<GameScreen> screensToUpdate = new List<GameScreen>();
		InputState input = new InputState();

		public ScreenManager(VectorGameSession game)
			: base(game) {
			input.Update();
			#if XBOX
			InputState.InputMethod = InputMethods.Gamepad;
			#else
			InputState.InputMethod = InputMethods.KeyboardMouse;
			#endif
		}

		public SpriteBatch SpriteBatch {
			get { return spriteBatch; }
		}
		SpriteBatch spriteBatch;

		public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer) {
			screen.ControllingPlayer = controllingPlayer;
			screen.ScreenManager = this;
			screens.Add(screen);
			screen.Initialize();
			screen.LoadContent(); // TODO: Move to loading screen
		}

		public void RemoveScreen() {
			RemoveScreen(screens[screens.Count - 1]);
			//screens[screens.Count - 1].ExitScreen();
		}

		public void RemoveScreen(GameScreen screen) {
			screen.UnloadContent();
			screens.Remove(screen);
			//screen.ExitScreen();
		}

		public void RemoveScreenAt(int index) {
			screens[index].UnloadContent();
			screens.RemoveAt(index);
		}

		public void ReplaceScreen(GameScreen screen, PlayerIndex? controllingPlayer) {
			//if (screens.Count > 0) RemoveScreen(screens[screens.Count - 1]);
			if (screens.Count > 0) screens[screens.Count - 1].ExitScreen();
			AddScreen(screen, controllingPlayer);
		}

		public void ReplaceAllScreens(GameScreen screen, PlayerIndex? controllingPlayer) {
			ClearScreens();
			AddScreen(screen, controllingPlayer);
		}

		public void ReplaceScreenProxy(GameScreen now, GameScreen after, PlayerIndex? controllingPlayer) {
			ReplaceScreen(new ScreenProxy(now, after), controllingPlayer);
		}

		public void ClearScreens() {
			screens.Clear();
		}

		public GameScreen[] GetScreens() {
			return screens.ToArray();
		}

		public GameScreen LastNonExiting() {
			GameScreen screen = null;
			foreach (GameScreen s in screens) {
				if (!s.IsExiting) screen = s;
			}
			return screen;
		}

		public GameScreen Last() {
			GameScreen[] screens = GetScreens();
			return screens[screens.Length - 1];
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		public override void Update(GameTime gameTime) {
			input.Update();

			//screens[screens.Count - 1].HandleInput(input);
			//screens[screens.Count - 1].Update(gameTime);
			foreach (GameScreen screen in screens)
				screensToUpdate.Add(screen);

			bool otherScreenHasFocus = !Game.IsActive;
			//bool coveredByOtherScreen = false;

			// Loop as long as there are screens waiting to be updated.
			while (screensToUpdate.Count > 0) {
				// Pop the topmost screen off the waiting list.
				GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

				screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

				// Update the screen.
				//screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
				screen.Update(gameTime);

				if (screen.ScreenState == ScreenState.TransitionOn ||
				    screen.ScreenState == ScreenState.Active) {
					// If this is the first active screen we came across,
					// give it a chance to handle input.
					if (!otherScreenHasFocus) {
						screen.HandleInput(gameTime, input);

						otherScreenHasFocus = true;
					}
				}
			}
		}

		public override void Draw(GameTime gameTime) {
			VGame.Renderer.BeginDrawing();
			foreach (GameScreen screen in screens) {
				if (screen.ScreenState == ScreenState.Hidden) continue;
				screen.Draw(gameTime);
			}
			((VectorGameSession)Game).DrawVectors(gameTime);
			VGame.Renderer.EndDrawing();
		}
	}
}
