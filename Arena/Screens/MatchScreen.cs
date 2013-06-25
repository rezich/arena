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
		int edgeScrollSize = 32;

		int viewportWidth {
			get {
				return Renderer.Width - HUD.BoxWidth * 2;
			}
		}
		int viewportHeight {
			get {
				return Renderer.Height;
			}
		}
		Vector2 cursorWorldPosition {
			get {
				return cursorPosition + viewPosition - viewOrigin;
			}
		}
		int viewMoveSpeed = 8;

		public MatchScreen() {
			LocalPlayer = new Player("takua108", 17, Teams.Home, Roles.Runner);
			LocalPlayer.JumpTo(new Vector2(144, 144));
			LocalPlayer.LevelUp(0);
			LocalPlayer.LevelUp(0);
			//LocalPlayer.LevelUp(2);
			//LocalPlayer.LevelUp(3);

			Bot bot1 = new Bot(Teams.Away, Roles.Nuker);
			bot1.JumpTo(new Vector2(400, 400));

			Bot bot2 = new Bot(Teams.Home, Roles.Grappler);
			bot2.JumpTo(new Vector2(200, 400));

			foreach (Player p in Player.List)
				p.MakeActor();
			viewPosition = new Vector2(0, 0);
			HUD.Recalculate();
		}
		public override void HandleInput(InputState input) {
			cursorPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			if (cursorPosition.X < viewOrigin.X + edgeScrollSize)
				viewPosition.X -= viewMoveSpeed;
			if (cursorPosition.X > viewOrigin.X + viewportWidth - edgeScrollSize)
				viewPosition.X += viewMoveSpeed;
			if (cursorPosition.Y < viewOrigin.Y + edgeScrollSize)
				viewPosition.Y -= viewMoveSpeed;
			if (cursorPosition.Y > viewOrigin.Y + viewportHeight - edgeScrollSize)
				viewPosition.Y += viewMoveSpeed;
			if (input.IsNewMousePress(MouseButtons.Right) && cursorPosition.X > HUD.BoxWidth && cursorPosition.X < Resolution.Width - HUD.BoxWidth) {
				Actor clickedActor = null;
				foreach (Actor a in Actor.List) {
					if (Vector2.Distance(cursorPosition, a.Position) < Arena.GameSession.ActorScale) {
						if (LocalPlayer.AttitudeTowards(a.Player) == Attitude.Enemy) {
							clickedActor = a;
							break;
						}
					}
				}
				if (clickedActor != null)
					LocalPlayer.AttackTarget = clickedActor;
				else {
					LocalPlayer.AttackTarget = null;
					LocalPlayer.IntendedPosition = cursorWorldPosition;
				}
			}
			if (input.IsKeyDown(Keys.Up))
				viewPosition.Y -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Left))
				viewPosition.X -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Right))
				viewPosition.X += viewMoveSpeed;
			if (input.IsKeyDown(Keys.Down))
				viewPosition.Y += viewMoveSpeed;
			if (input.IsNewKeyPress(Keys.Q)) {
				LocalPlayer.UseAbility(0);
			}
			if (input.IsNewKeyPress(Keys.W)) {
				LocalPlayer.UseAbility(1);
			}
			if (input.IsNewKeyPress(Keys.E)) {
				LocalPlayer.UseAbility(2);
			}
			if (input.IsNewKeyPress(Keys.R)) {
				LocalPlayer.UseAbility(3);
			}
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

			HUD.Draw(gameTime, g, LocalPlayer);

			cursor.Draw(g, cursorPosition, 0, LocalPlayer.AttackTarget == null ? new Cairo.Color(1, 1, 1) : new Cairo.Color(0, 0, 1), new Cairo.Color(0.1, 0.1, 0.1), 22);
		}
	}
}

