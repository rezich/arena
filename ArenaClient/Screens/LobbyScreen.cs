using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class LobbyScreen : State {
		public LobbyScreen() {
		}
		public override void HandleInput(GameTime gameTime) {
			if (Client.Local.IsChatting) {
			}
			else {
				if (InputManager.KeyState(Keys.Q) == ButtonState.Pressed) {
					Client.Local.ChangeTeam(Teams.Home);
				}
				if (InputManager.KeyState(Keys.W) == ButtonState.Pressed) {
					Client.Local.ChangeTeam(Teams.Away);
				}
				if (InputManager.KeyState(Keys.E) == ButtonState.Pressed) {
					Client.Local.ChangeTeam(Teams.Spectator);
				}
				if (InputManager.KeyState(Keys.R) == ButtonState.Pressed) {
					Client.Local.ToggleReady();
				}
				if (InputManager.KeyState(Keys.D1) == ButtonState.Pressed) {
					Client.Local.ChangeRole(Roles.Runner);
				}
				if (InputManager.KeyState(Keys.D2) == ButtonState.Pressed) {
					Client.Local.ChangeRole(Roles.Nuker);
				}
				if (InputManager.KeyState(Keys.D3) == ButtonState.Pressed) {
					Client.Local.ChangeRole(Roles.Grappler);
				}
				if (InputManager.KeyState(Keys.D4) == ButtonState.Pressed) {
					Client.Local.ChangeRole(Roles.Tank);
				}
			}
		}
		public override void Update(GameTime gameTime) {
			if (Client.Local.IsLocalServer)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, Vector2.Zero, Vector2.Zero);
			if (Client.Local.Match != null) {
				Console.WriteLine("[C] Moving to MatchLoadingScreen...");
				StateManager.ReplaceState(new MatchLoadingScreen());
			}
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = Renderer.Context;

			Renderer.Clear(new Color(0.83, 0.83, 0.83));

			Vector2 neutralOrigin = new Vector2(Renderer.Width / 2, 8);
			Vector2 homeOrigin = neutralOrigin - new Vector2(Renderer.Width / 3, 0);
			Vector2 awayOrigin = neutralOrigin + new Vector2(Renderer.Width / 3, 0);
			Vector2 spectatorOrigin = neutralOrigin + new Vector2(0, 200);

			List<string> help = new List<string>() {
				"Q: move to HOME",
				"W: move to AWAY",
				"E: move to SPECTATOR",
				"R: toggle READY",
				"1: change to RUNNER",
				"2: change to NUKER",
				"3: change to GRAPPLER",
				"4: change to TANK"
			};

			for (int i = 0; i < help.Count; i++)
				Renderer.DrawText(new Vector2(4, 4 + i * 14), help[i], 14, TextAlign.Left, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);

			// HOME
			Renderer.DrawText(homeOrigin, "HOME", 20, TextAlign.Left, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.HomeColor2, 0, null);
			int offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Home)) {
				Renderer.DrawText(homeOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name + " - " + kvp.Value.Role, 20, TextAlign.Left, TextAlign.Top, kvp.Value.Ready ? Config.HomeColor1 : HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// UNASSIGNED
			Renderer.DrawText(neutralOrigin, "UNASSIGNED", 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.NeutralColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Neutral)) {
				Renderer.DrawText(neutralOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name, 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// AWAY
			Renderer.DrawText(awayOrigin, "AWAY", 20, TextAlign.Right, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.AwayColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Away)) {
				Renderer.DrawText(awayOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Role + " - " + kvp.Value.Name, 20, TextAlign.Right, TextAlign.Top, kvp.Value.Ready ? Config.AwayColor1 : HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// SPECTATOR
			Renderer.DrawText(spectatorOrigin, "SPECTATOR", 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.NeutralColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Spectator)) {
				Renderer.DrawText(spectatorOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name, 20, TextAlign.Center, TextAlign.Top, kvp.Value.Ready ? Config.NeutralColor1 : HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			base.Draw(gameTime);
		}
	}
}

