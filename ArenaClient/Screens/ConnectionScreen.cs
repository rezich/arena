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
			Client.Local = new Client(false);
			Client.Local.Connect();
		}

		public override void Update(GameTime gameTime) {
			Client.Local.Update(gameTime, Vector2.Zero, Vector2.Zero);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
		}
	}
}

