using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;

namespace Arena {
	public class MatchScreen : VGame.GameScreen {
		Player LocalPlayer;
		public MatchScreen() {
			LocalPlayer = new Player("takua108", 17, Teams.Home, Roles.Runner);
			LocalPlayer.JumpTo(new Vector2(300, 300));

			foreach (Player p in Player.List)
				p.MakeActor();
		}
		public override void HandleInput(InputState input) {
			if (input.CurrentMouseState.RightButton == ButtonState.Pressed) {
				LocalPlayer.IntendedPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			}
			base.HandleInput(input);
		}
		public override void Update(GameTime gameTime) {
			foreach (Player p in Player.List)
				p.Update(gameTime);
			foreach (Actor a in Actor.List)
				a.Update(gameTime);
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = VGame.Renderer.Context;

			int gridSize = Convert.ToInt32(Arena.GameSession.ActorScale);
			for (int i = 0; i < (int)Math.Floor((double)Resolution.Width / (double)gridSize); i++) {
				g.MoveTo(i * gridSize, 0);
				g.LineTo(i * gridSize, Resolution.Height);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			for (int i = 0; i < (int)Math.Floor((double)Resolution.Height / (double)gridSize); i++) {
				g.MoveTo(0, i * gridSize);
				g.LineTo(Resolution.Width, i * gridSize);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			foreach (Actor a in Actor.List) {
				a.DrawUIBelow(gameTime, g);
			}
			foreach (Actor a in Actor.List) {
				a.Draw(gameTime, g);
			}
			foreach (Actor a in Actor.List) {
				a.DrawUIAbove(gameTime, g);
			}

			((IDisposable)g).Dispose();
		}
	}
}

