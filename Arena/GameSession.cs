#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion
namespace Arena {
	/// <summary>
    /// This is the main type for your game
    /// </summary>
	public class GameSession : VGame.VectorGameSession {
		public static GameSession Current = null;

		public static Cairo.Color HomeColor1;
		public static Cairo.Color HomeColor2;
		public static Cairo.Color AwayColor1;
		public static Cairo.Color AwayColor2;
		public static Cairo.Color HealthColor1;
		public static Cairo.Color HealthColor2;
		public static Cairo.Color EnemyHealthColor1;
		public static Cairo.Color EnemyHealthColor2;
		public static Cairo.Color EnergyColor1;
		public static Cairo.Color EnergyColor2;
		public static Cairo.Color HUDBackground;
		public static Cairo.Color HUDText;
		public static double ActorScale = 24;
		public static Random Random = new Random();

		public static Teams CurrentTeam;
		
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
		protected override void Initialize() {
			HomeColor1 = VGame.Util.MakeColor(152, 174, 188, 1);
			HomeColor2 = VGame.Util.MakeColor(92, 123, 142, 1);
			AwayColor1 = VGame.Util.MakeColor(242, 159, 98, 1);
			AwayColor2 = VGame.Util.MakeColor(238, 120, 30, 1);
			HealthColor1 = VGame.Util.MakeColor(16, 128, 16, 0.1);
			HealthColor2 = VGame.Util.MakeColor(8, 128, 8, 0.15);
			EnemyHealthColor1 = HealthColor1; //VGame.Util.MakeColor(128, 16, 16, 0.2);
			EnemyHealthColor2 = HealthColor2; //VGame.Util.MakeColor(128, 8, 8, 0.25);
			EnergyColor1 = VGame.Util.MakeColor(32, 32, 128, 0.2);
			EnergyColor2 = VGame.Util.MakeColor(16, 16, 128, 0.25);
			HUDBackground = VGame.Util.MakeColor(48, 48, 48, 1);
			HUDText = VGame.Util.MakeColor(255, 255, 255, 1);
			IsMouseVisible = false;

			Role.Initialize();

			screenManager.AddScreen(new MatchScreen(), PlayerIndex.One);

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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit();
			}

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

