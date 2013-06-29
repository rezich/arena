using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class ConnectionScreen : VGame.GameScreen {
		public ConnectionScreen() {
			if (Arena.Config.LocalServer) {
				new Server(true);
			}
			Client.Local = new Client(Arena.Config.LocalServer);
			Client.Local.Connect();
		}

		public override void Update(GameTime gameTime) {
			if (Client.Local.IsLocalServer)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, Vector2.Zero, Vector2.Zero);
			if (Client.Local.IsConnected) {
				Console.WriteLine("Moving to MatchScreen...");
				ScreenManager.ReplaceScreen(new MatchScreen(), PlayerIndex.One);
			}
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
		}
	}
}

