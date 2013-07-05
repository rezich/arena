#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Arena;

#endregion
namespace ArenaClient {
	/// <summary>
    /// This is the main type for your game
    /// </summary>
	public class GameSession : VGame.VectorGameSession {
		public static GameSession Current = null;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
		protected override void Initialize() {
			Current = this;
			IsMouseVisible = false;

			Role.Initialize();
			Arena.Config.Initialize();
			HUD.Recalculate();

			//screenManager.AddScreen(new MatchScreen(), PlayerIndex.One);
			//screenManager.AddScreen(new ConnectionScreen(), PlayerIndex.One);
			screenManager.AddScreen(new MainMenu(), null);

			base.Initialize();
		}
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
		protected override void LoadContent() {
			base.LoadContent();
		}
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			/*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) && (Client.Local == null || Client.Local.IsChatting == false) )) {
				Exit();
			}*/

			base.Update(gameTime);
		}
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
		}
	}
}

