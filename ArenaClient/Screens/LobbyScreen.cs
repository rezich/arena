using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class LobbyScreen : GameScreen {
		public LobbyScreen() {
		}
		public override void HandleInput(GameTime gameTime, InputState input) {
			if (Client.Local.IsChatting) {
			}
			else {
				if (input.IsNewKeyPress(Config.KeyBindings[KeyCommand.UnlockCursor])) {
					ScreenManager.Game.IsMouseVisible = !ScreenManager.Game.IsMouseVisible;
				}
				if (input.IsNewKeyPress(Config.KeyBindings[KeyCommand.Ability1])) {
					Client.Local.ChangeTeam(Teams.Home);
				}
				if (input.IsNewKeyPress(Config.KeyBindings[KeyCommand.Ability2])) {
					Client.Local.ChangeTeam(Teams.Away);
				}
				if (input.IsNewKeyPress(Config.KeyBindings[KeyCommand.Ability3])) {
					Client.Local.ChangeTeam(Teams.Spectator);
				}
				if (input.IsNewKeyPress(Keys.D1)) {
					Client.Local.ChangeRole(Roles.Runner);
				}
				if (input.IsNewKeyPress(Keys.D2)) {
					Client.Local.ChangeRole(Roles.Nuker);
				}
				if (input.IsNewKeyPress(Keys.D3)) {
					Client.Local.ChangeRole(Roles.Grappler);
				}
				if (input.IsNewKeyPress(Keys.D4)) {
					Client.Local.ChangeRole(Roles.Tank);
				}
			}
			base.HandleInput(gameTime, input);
		}
		public override void Update(GameTime gameTime) {
			if (Client.Local.IsLocalServer)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, Vector2.Zero, Vector2.Zero);
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = VGame.Renderer.Context;

			Vector2 neutralOrigin = new Vector2(Renderer.Width / 2, 8);
			Vector2 homeOrigin = neutralOrigin - new Vector2(Renderer.Width / 3, 0);
			Vector2 awayOrigin = neutralOrigin + new Vector2(Renderer.Width / 3, 0);
			Vector2 spectatorOrigin = neutralOrigin + new Vector2(0, 200);

			List<string> help = new List<string>() {
				"Q: move to HOME",
				"W: move to AWAY",
				"E: move to SPECTATOR",
				"1: change to RUNNER",
				"2: change to NUKER",
				"3: change to GRAPPLER",
				"4: change to TANK"
			};

			for (int i = 0; i < help.Count; i++)
				HUD.DrawText(g, new Vector2(4, 4 + i * 14), help[i], 14, TextAlign.Left, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);

			// HOME
			HUD.DrawText(g, homeOrigin, "HOME", 20, TextAlign.Left, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.HomeColor2, 0, null);
			int offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Home)) {
				HUD.DrawText(g, homeOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name + " - " + kvp.Value.Role, 20, TextAlign.Left, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// UNASSIGNED
			HUD.DrawText(g, neutralOrigin, "UNASSIGNED", 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.NeutralColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Neutral)) {
				HUD.DrawText(g, neutralOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name, 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// AWAY
			HUD.DrawText(g, awayOrigin, "AWAY", 20, TextAlign.Right, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.AwayColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Away)) {
				HUD.DrawText(g, awayOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Role + " - " + kvp.Value.Name, 20, TextAlign.Right, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
			// SPECTATOR
			HUD.DrawText(g, spectatorOrigin, "SPECTATOR", 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, Config.NeutralColor2, 0, null);
			offset = 0;
			foreach (KeyValuePair<int, Player> kvp in Client.Local.Players.Where(x => x.Value.Team == Teams.Spectator)) {
				HUD.DrawText(g, spectatorOrigin + new Vector2(0, 20 * (offset + 1)), kvp.Value.Name, 20, TextAlign.Center, TextAlign.Top, HUD.MainTextFill, HUD.MainTextStroke, null, 0, null);
				offset++;
			}
		}
	}
}

