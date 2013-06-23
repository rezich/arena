using System;
using Microsoft.Xna.Framework;
using Cairo;
using VGame;

namespace Arena {
	public class MatchScreen : VGame.GameScreen {
		public MatchScreen() {
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = VGame.Renderer.Context;

			int gridSize = 32;
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

			g.MoveTo(0, 0);
			g.LineTo(Resolution.Width, Resolution.Height);
			g.Color = new Cairo.Color(0, 0, 1);
			g.Stroke();

			((IDisposable)g).Dispose();
		}
	}
}

