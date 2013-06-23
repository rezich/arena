using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Cairo;
using VGame;

namespace Arena {
	public class MatchScreen : VGame.GameScreen {
		public MatchScreen() {
			Player.List.Add(new Player("takua108", 17, Teams.Home, Roles.Runner));
			Player.List[0].Position = new Vector2(108, 108);

			foreach (Player p in Player.List)
				p.MakeActor();
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

