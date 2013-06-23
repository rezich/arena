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
		Vector2 cursorPosition;
		Shapes.Cursor cursor = new Arena.Shapes.Cursor();

		public MatchScreen() {
			LocalPlayer = new Player("takua108", 17, Teams.Home, Roles.Runner);
			LocalPlayer.JumpTo(new Vector2(300, 300));

			foreach (Player p in Player.List)
				p.MakeActor();
		}
		public override void HandleInput(InputState input) {
			cursorPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			if (input.IsNewMousePress(MouseButtons.Right)) {
				LocalPlayer.IntendedPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			}
			PlayerIndex playerIndex;
			if (input.IsNewKeyPress(Keys.Up, null, out playerIndex))
				LocalPlayer.Health = Math.Min(LocalPlayer.Health + 1, LocalPlayer.MaxHealth);
			if (input.IsNewKeyPress(Keys.Down, null, out playerIndex))
				LocalPlayer.Health = Math.Max(LocalPlayer.Health - 1, 0);
			if (input.IsNewKeyPress(Keys.Right, null, out playerIndex))
				LocalPlayer.Energy = Math.Min(LocalPlayer.Energy + 1, LocalPlayer.MaxEnergy);
			if (input.IsNewKeyPress(Keys.Left, null, out playerIndex))
				LocalPlayer.Energy = Math.Max(LocalPlayer.Energy - 1, 0);
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

			DrawHUD(gameTime, g);

			cursor.Draw(g, cursorPosition, 0, new Cairo.Color(1, 1, 1), new Cairo.Color(0.1, 0.1, 0.1), 22);

			((IDisposable)g).Dispose();
		}
		protected void DrawHUD(GameTime gameTime, Cairo.Context g) {
			int width = Resolution.Width / 10;
			int margin = 4;
			double barWidth = (width - 4 * margin) / 3;
			double minimapSize = (width - 2 * margin);
			double barHeight = Resolution.Height - margin * 4 - minimapSize * 2;

			// LEFT
			g.MoveTo(Resolution.Left, Resolution.Top);
			g.LineTo(Resolution.Left + width, Resolution.Top);
			g.LineTo(Resolution.Left + width, Resolution.Bottom);
			g.LineTo(Resolution.Left, Resolution.Bottom);
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, Arena.GameSession.HUDBackground, null);

			// health background
			g.MoveTo(new Vector2(Resolution.Left + margin, Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + barWidth), Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2(Resolution.Left + margin, Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// health foreground
			g.MoveTo(new Vector2(Resolution.Left + margin, Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.HealthPercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.HealthPercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2(Resolution.Left + margin, Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 128, 0), null);

			// energy background
			g.MoveTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth), Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth * 2), Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// energy foreground
			g.MoveTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.EnergyPercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.EnergyPercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 2 + barWidth), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(128, 0, 0), null);

			// experience background
			g.MoveTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 2), Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 3), Resolution.Top + margin * 2 + (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 3), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// experience foreground
			g.MoveTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.ExperiencePercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 3), Resolution.Bottom - margin * 2 - (float)minimapSize - (float)(barHeight * LocalPlayer.ExperiencePercent)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 3), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin * 3 + barWidth * 2), Resolution.Bottom - margin * 2 - (float)minimapSize).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 128, 128), null);

			// minimap
			g.MoveTo(new Vector2((float)(Resolution.Left + margin), (float)(Resolution.Bottom - margin)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + minimapSize), (float)(Resolution.Bottom - margin)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin + minimapSize), (float)(Resolution.Bottom - margin - minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Left + margin), (float)(Resolution.Bottom - margin - minimapSize)).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);


			// RIGHT
			g.MoveTo(Resolution.Right, Resolution.Top);
			g.LineTo(Resolution.Right - width, Resolution.Top);
			g.LineTo(Resolution.Right - width, Resolution.Bottom);
			g.LineTo(Resolution.Right, Resolution.Bottom);
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, Arena.GameSession.HUDBackground, null);

			DrawText(g, new Vector2(Resolution.Left + margin, Resolution.Top + margin), "LEVEL: 2");
			DrawText(g, new Vector2(Resolution.Left + margin, Resolution.Top + margin + 16), "GOLD: 108");
		}
		protected void DrawText(Cairo.Context g, Vector2 position, string text) {
			DrawText(g, position, text, 0);
		}
		protected void DrawText(Cairo.Context g, Vector2 position, string text, double angle) {
			g.Save();
			g.SelectFontFace("04b_19", FontSlant.Normal, FontWeight.Normal);
			double textScale = 16;
			g.SetFontSize(textScale);
			TextExtents ext = g.TextExtents(text);
			Vector2 textPos = new Vector2((float)(position.X - ext.XBearing), (float)(position.Y - ext.YBearing));
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(1, 1, 1, 1);
			if (angle != 0) g.Rotate(angle);
			g.ShowText(text);
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(0, 0, 0, 0.8);
			g.LineWidth = 1;
			g.TextPath(text);
			if (angle != 0) g.Rotate(angle);
			g.Stroke();
			g.Restore();
		}
	}
}

