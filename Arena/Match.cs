using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arena {
	public class Match {
		public List<Actor> Actors;
		public List<Player> Players;
		public Match() {
		}
		public void Draw(GraphicsDeviceManager graphics, Cairo.Context g) {
			int gridSize = 32;
			for (int i = 0; i < (int)Math.Floor((double)graphics.PreferredBackBufferWidth / (double)gridSize); i++) {
				g.MoveTo(i * gridSize, 0);
				g.LineTo(i * gridSize, graphics.PreferredBackBufferHeight);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			for (int i = 0; i < (int)Math.Floor((double)graphics.PreferredBackBufferHeight / (double)gridSize); i++) {
				g.MoveTo(0, i * gridSize);
				g.LineTo(graphics.PreferredBackBufferWidth, i * gridSize);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			foreach (Actor a in Actors) {
				a.DrawUIBelow(g);
			}
			foreach (Actor a in Actors) {
				a.Draw(g);
			}
			foreach (Actor a in Actors) {
				a.DrawUIAbove(g);
			}
		}
	}
}

