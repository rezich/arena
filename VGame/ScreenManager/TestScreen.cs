using System;
using Microsoft.Xna.Framework;
using Cairo;

namespace VGame {
	public class TestScreen : GameScreen {
		public TestScreen() {
		}
		public override void Draw(GameTime gameTime) {
			if (VGame.Renderer.Context == null) {
				throw new Exception("wtf");
			}
			Cairo.Context g = VGame.Renderer.Context;

			/*int gridSize = 32;
			for (int i = 0; i < (int)Math.Floor((double)Resolution.Right / (double)gridSize); i++) {
				g.MoveTo(i * gridSize, 0);
				g.LineTo(i * gridSize, Resolution.Bottom);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			for (int i = 0; i < (int)Math.Floor((double)Resolution.Bottom / (double)gridSize); i++) {
				g.MoveTo(0, i * gridSize);
				g.LineTo(Resolution.Right, i * gridSize);
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}*/
			g.MoveTo(0, 0);
			g.LineTo(400, 400);
			g.Color = new Cairo.Color(0, 0, 1);
			g.Stroke();
			g.MoveTo(0, 0);
			g.LineTo(200, 400);
			g.Color = new Cairo.Color(0, 0, 1);
			g.Stroke();
			g.MoveTo(0, 0);
			g.LineTo(400, 200);
			g.Color = new Cairo.Color(0, 0, 1);
			g.Stroke();

			((IDisposable)g).Dispose();
		}
	}
}

