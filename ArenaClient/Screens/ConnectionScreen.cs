using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class ConnectionScreen : GameScreen {
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
			if (Client.Local.IsConnected && Client.Local.LocalPlayer != null) {
				// TODO: Add support to move straight to MatchScreen if reconnecting
				ScreenManager.ReplaceScreen(new LobbyScreen(), PlayerIndex.One);
			}
			// TODO: Add retries and timeouts and stuff
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
		}
	}
}

