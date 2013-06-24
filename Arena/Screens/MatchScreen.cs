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
		Vector2 viewPosition;
		Vector2 viewOrigin;
		int hudWidth;
		int viewportWidth {
			get {
				return Renderer.Width - hudWidth * 2;
			}
		}
		int viewMoveSpeed = 8;

		public MatchScreen() {
			LocalPlayer = new Player("takua108", 17, Teams.Home, Roles.Runner);
			LocalPlayer.JumpTo(new Vector2(144, 144));

			Bot bot = new Bot(Teams.Away, Roles.Nuker);
			bot.JumpTo(new Vector2(400, 400));

			foreach (Player p in Player.List)
				p.MakeActor();
			viewPosition = new Vector2(0, 0);
			hudWidth = Resolution.Width / 10;
		}
		public override void HandleInput(InputState input) {
			cursorPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			if (input.IsNewMousePress(MouseButtons.Right) && cursorPosition.X > hudWidth && cursorPosition.X < Resolution.Width - hudWidth) {
				LocalPlayer.IntendedPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y) + viewPosition - viewOrigin;
			}
			PlayerIndex playerIndex;
			/*if (input.IsNewKeyPress(Keys.Up, null, out playerIndex))
				LocalPlayer.Health = Math.Min(LocalPlayer.Health + 1, LocalPlayer.MaxHealth);
			if (input.IsNewKeyPress(Keys.Down, null, out playerIndex))
				LocalPlayer.Health = Math.Max(LocalPlayer.Health - 1, 0);
			if (input.IsNewKeyPress(Keys.Right, null, out playerIndex))
				LocalPlayer.Energy = Math.Min(LocalPlayer.Energy + 1, LocalPlayer.MaxEnergy);
			if (input.IsNewKeyPress(Keys.Left, null, out playerIndex))
				LocalPlayer.Energy = Math.Max(LocalPlayer.Energy - 1, 0);*/
			if (input.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Up))
				viewPosition.Y -= viewMoveSpeed;
			if (input.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Left))
				viewPosition.X -= viewMoveSpeed;
			if (input.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Right))
				viewPosition.X += viewMoveSpeed;
			if (input.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Down))
				viewPosition.Y += viewMoveSpeed;
			base.HandleInput(input);
		}
		public override void Update(GameTime gameTime) {
			foreach (Player p in Player.List)
				p.Update(gameTime);
			foreach (Actor a in Actor.List)
				a.Update(gameTime, viewPosition, viewOrigin);
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = VGame.Renderer.Context;

			viewOrigin = new Vector2(Renderer.Width / 10, 0);
			Vector2 gridOffset = new Vector2((float)(viewPosition.X % Arena.GameSession.ActorScale), (float)(viewPosition.Y % Arena.GameSession.ActorScale));
			int gridSize = Convert.ToInt32(Arena.GameSession.ActorScale);
			int gridWidth = viewportWidth + 2 * gridSize;
			int gridHeight = Resolution.Height + 2 * gridSize;

			for (int i = 0; i < ((int)Math.Floor((double)gridWidth / (double)gridSize)); i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(0, gridSize) + new Vector2(i * gridSize, 0)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(0, gridSize) + new Vector2(i * gridSize, Resolution.Height)).ToPointD());
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			for (int i = 0; i < ((int)Math.Floor((double)gridHeight / (double)gridSize)); i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(gridSize, 0) + new Vector2(0, i * gridSize)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(gridSize, 0) + new Vector2(viewportWidth, i * gridSize)).ToPointD());
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

			//((IDisposable)g).Dispose();
		}
		protected void DrawHUD(GameTime gameTime, Cairo.Context g) {
			int margin = 4;
			double barWidth = (hudWidth - 4 * margin) / 3;
			double minimapSize = (hudWidth - 2 * margin);
			double barHeight = Resolution.Height - margin * 4 - minimapSize * 2;

			// LEFT
			g.MoveTo(Resolution.Left, Resolution.Top);
			g.LineTo(Resolution.Left + hudWidth, Resolution.Top);
			g.LineTo(Resolution.Left + hudWidth, Resolution.Bottom);
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
			g.LineTo(Resolution.Right - hudWidth, Resolution.Top);
			g.LineTo(Resolution.Right - hudWidth, Resolution.Bottom);
			g.LineTo(Resolution.Right, Resolution.Bottom);
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, Arena.GameSession.HUDBackground, null);

			// skill 1
			g.MoveTo(new Vector2(Resolution.Right - margin, (float)(Renderer.Height / 2 - 3 * margin / 2 - minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 - 3 * margin / 2 - minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 - 3 * margin / 2 - minimapSize * 2)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin), (float)(Renderer.Height / 2 - 3 * margin / 2 - minimapSize * 2)).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// skill 2
			g.MoveTo(new Vector2(Resolution.Right - margin, Renderer.Height / 2 - margin / 2).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 - margin / 2)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 - margin / 2 - minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin), (float)(Renderer.Height / 2 - margin / 2 - minimapSize)).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// skill 3
			g.MoveTo(new Vector2(Resolution.Right - margin, Renderer.Height / 2 + margin / 2).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 + margin / 2)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 + margin / 2 + minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin), (float)(Renderer.Height / 2 + margin / 2 + minimapSize)).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			// skill 4
			g.MoveTo(new Vector2(Resolution.Right - margin, (float)(Renderer.Height / 2 + 3 * margin / 2 + minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 + 3 * margin / 2 + minimapSize)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin - minimapSize), (float)(Renderer.Height / 2 + 3 * margin / 2 + minimapSize * 2)).ToPointD());
			g.LineTo(new Vector2((float)(Resolution.Right - margin), (float)(Renderer.Height / 2 + 3 * margin / 2 + minimapSize * 2)).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, new Cairo.Color(0, 0, 0), null);

			DrawText(g, new Vector2(Resolution.Left + hudWidth + margin, Resolution.Top + margin), Renderer.FPS.ToString(), 20, 0);
			DrawText(g, new Vector2(Resolution.Left + margin, Resolution.Top + margin), "LEVEL: 2", 14, 0);
			DrawText(g, new Vector2(Resolution.Left + margin, Resolution.Top + margin + 14), "GOLD: 108", 14, 0);

			DrawText(g, new Vector2(Resolution.Right - hudWidth + margin, Resolution.Top + margin), LocalPlayer.Abilities[0].Name, 14, 0);
			DrawText(g, new Vector2(Resolution.Right - hudWidth + margin, Resolution.Top + margin + 14), LocalPlayer.Abilities[1].Name, 14, 0);
			DrawText(g, new Vector2(Resolution.Right - hudWidth + margin, Resolution.Top + margin + 28), LocalPlayer.Abilities[2].Name, 14, 0);
			DrawText(g, new Vector2(Resolution.Right - hudWidth + margin, Resolution.Top + margin + 42), LocalPlayer.Abilities[3].Name, 14, 0);
		}
		protected void DrawText(Cairo.Context g, Vector2 position, string text, double scale, double angle) {
			g.Save();
			g.Antialias = Antialias.None;
			g.SelectFontFace("04b_19", FontSlant.Normal, FontWeight.Normal);
			g.SetFontSize(scale);
			TextExtents ext = g.TextExtents(text);
			Vector2 textPos = new Vector2((float)(position.X - ext.XBearing), (float)(position.Y - ext.YBearing));
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(1, 1, 1, 1);
			if (angle != 0) g.Rotate(angle);
			g.ShowText(text);
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(0, 0, 0, 1);
			g.LineWidth = 1;
			g.TextPath(text);
			if (angle != 0) g.Rotate(angle);
			g.Stroke();
			g.Restore();
		}
	}
}

