using System;
using System.Collections;
using System.Collections.Generic;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class ConnectionScreen : State {
		string address;

		public ConnectionScreen(string address) : base() {
			this.address = address;
		}

		public override void Initialize() {
			if (address == "") {
				new Server(true);
				Client.Local = new Client(StateManager.Game, true);
				Client.Local.Connect();
			}
			else {
				Client.Local = new Client(StateManager.Game, false);
				Client.Local.Connect(address);
			}
		}

		public override void Update(GameTime gameTime) {
			if (Client.Local.IsLocalServer)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, Vector2.Zero, Vector2.Zero);
			if (Client.Local.IsConnected && Client.Local.LocalPlayer != null) {
				// TODO: Add support to move straight to MatchScreen if reconnecting
				StateManager.ReplaceAllStates(new LobbyScreen());
			}
			// TODO: Add retries and timeouts and stuff
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);
		}
	}
}

