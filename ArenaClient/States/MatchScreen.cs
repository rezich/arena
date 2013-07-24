using System;
using System.Collections;
using System.Collections.Generic;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MatchScreen : State {
		Vector2 cursorPosition;
		Arena.Shapes.Cursor cursor = new Arena.Shapes.Cursor();
		int edgeScrollSize = 32;
		double markerAnimationPercent = 0;
		TimeSpan markerAnimationDuration = TimeSpan.FromSeconds(0.25);
		TimeSpan markerAnimationDone = TimeSpan.Zero;

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
				Vector2 pos = cursorPosition + Client.Local.ViewPosition - Client.Local.ViewOrigin;
				return new Vector2((float)(pos.X / Game.Zoom), (float)(pos.Y / Game.Zoom));
			}
		}
		int viewMoveSpeed = 16;

		bool isLocalGame = false;

		public override void Initialize() {
			HUD.Recalculate(Renderer);
		}
		public override void OnFocus() {
			Game.ConstrainMouse = true;
		}
		public override void OnEscape() {
			Game.Cmd.Run("pausemenu");
		}
		public override void HandleInput(GameTime gameTime) {
			cursorPosition = new Vector2(InputManager.MousePosition.X, InputManager.MousePosition.Y);
			if (cursorPosition.X < Client.Local.ViewOrigin.X + edgeScrollSize && !StateManager.Game.CursorVisible)
				Client.Local.ViewPosition.X -= viewMoveSpeed;
			if (cursorPosition.X > Client.Local.ViewOrigin.X + viewportWidth - edgeScrollSize && !StateManager.Game.CursorVisible)
				Client.Local.ViewPosition.X += viewMoveSpeed;
			if (cursorPosition.Y < Client.Local.ViewOrigin.Y + edgeScrollSize && !StateManager.Game.CursorVisible)
				Client.Local.ViewPosition.Y -= viewMoveSpeed;
			if (cursorPosition.Y > Client.Local.ViewOrigin.Y + viewportHeight - edgeScrollSize && !StateManager.Game.CursorVisible)
				Client.Local.ViewPosition.Y += viewMoveSpeed;
			if (InputManager.MouseButtonState(MouseButton.Right) == ButtonState.Pressed && cursorPosition.X > HUD.BoxWidth && cursorPosition.X < Renderer.Width - HUD.BoxWidth) {
				markerAnimationDone = gameTime.TotalGameTime + markerAnimationDuration;
				Actor clickedActor = null;
				foreach (Actor a in Client.Local.Actors) {
					if (Vector2.Distance(cursorPosition, a.Position) < Game.Renderer.GetUnitSize(Config.ActorSize)) {
						if (a.Unit.Owner != Client.Local.LocalPlayer && Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Enemy) {
							clickedActor = a;
							break;
						}
					}
				}
				if (clickedActor != null)
					Client.Local.SendAttackOrder(clickedActor.Unit);
				else
					Client.Local.SendMoveOrder(cursorWorldPosition);

			}

			if (!Client.Local.HandleChatInput(InputManager)) {
				if (InputManager.KeyState(Keys.Tab) == ButtonState.Pressed) {
					Game.Cmd.Run("+scoreboard");
				}
				if (InputManager.KeyState(Keys.Tab) == ButtonState.Released) {
					Game.Cmd.Run("-scoreboard");
				}
				if (InputManager.KeyDown(Keys.Up))
					Client.Local.ViewPosition.Y -= viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Left))
					Client.Local.ViewPosition.X -= viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Right))
					Client.Local.ViewPosition.X += viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Down))
					Client.Local.ViewPosition.Y += viewMoveSpeed;
				if (InputManager.KeyState(Keys.Q) == ButtonState.Pressed && InputManager.IsShiftKeyDown) {
					Client.Local.LevelUp(gameTime, 0);
				}
				if (InputManager.KeyState(Keys.W) == ButtonState.Pressed && InputManager.IsShiftKeyDown) {
					Client.Local.LevelUp(gameTime, 1);
				}
				if (InputManager.KeyState(Keys.E) == ButtonState.Pressed && InputManager.IsShiftKeyDown) {
					Client.Local.LevelUp(gameTime, 2);
				}
				if (InputManager.KeyState(Keys.R) == ButtonState.Pressed && InputManager.IsShiftKeyDown) {
					Client.Local.LevelUp(gameTime, 3);
				}
				if (InputManager.KeyState(Keys.T) == ButtonState.Pressed) {
					// Chatwheel
					Client.Local.SendTeamChat("Well played!");
				}
				if (InputManager.KeyState(Keys.Space) == ButtonState.Pressed) {
					Client.Local.ViewPosition = new Vector2((float)(Client.Local.LocalPlayer.CurrentUnit.Position.X * Game.Zoom), (float)(Client.Local.LocalPlayer.CurrentUnit.Position.Y * Game.Zoom)) - new Vector2(viewportWidth / 2, viewportHeight / 2);
				}
				if (InputManager.KeyState(Keys.D1) == ButtonState.Pressed) {
					Client.Local.LevelUp(gameTime, 0);
				}
				if (InputManager.KeyState(Keys.D2) == ButtonState.Pressed) {
					Client.Local.LevelUp(gameTime, 1);
				}
				if (InputManager.KeyState(Keys.D3) == ButtonState.Pressed) {
					Client.Local.LevelUp(gameTime, 2);
				}
				if (InputManager.KeyState(Keys.D4) == ButtonState.Pressed) {
					Client.Local.LevelUp(gameTime, 3);
				}
				if (InputManager.KeyDown(Keys.Z)) {
					Game.Zoom += 0.1;
				}
				if (InputManager.KeyDown(Keys.X)) {
					Game.Zoom = Math.Max(Game.Zoom - 0.05, 0.15);
				}
			}
		}
		public override void Update(GameTime gameTime) {
			if (isLocalGame)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, Client.Local.ViewPosition, Client.Local.ViewOrigin);
			HUD.Update(gameTime, Client.Local.LocalPlayer);

			markerAnimationPercent = Math.Min(Math.Max(((double)(markerAnimationDone.TotalMilliseconds - gameTime.TotalGameTime.TotalMilliseconds) / (double)markerAnimationDuration.TotalMilliseconds), 0), 1);
		}
		public override void Draw(GameTime gameTime) {
			if (Client.Local.LocalPlayer == null || Client.Local.LocalPlayer.CurrentUnit == null)
				return;
			Renderer.Clear(ColorPresets.Gray85);
			Cairo.Context g = Renderer.Context;

			Client.Local.ViewOrigin = new Vector2(Renderer.Width / 10, 0);
			int gridSize = Convert.ToInt32(Renderer.GetUnitSize(Config.GridSizeLarge));
			Vector2 gridOffset = new Vector2((float)(Client.Local.ViewPosition.X % gridSize), (float)(Client.Local.ViewPosition.Y % gridSize));
			int gridWidth = viewportWidth + 2 * gridSize;
			int gridHeight = Renderer.Height + 2 * gridSize;

			// Minor grid
			/*for (int i = 0; i < ((int)Math.Floor((double)gridWidth / (double)gridSize)) * 4; i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(0, gridSize) + new Vector2(i * gridSize / 4, 0)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(0, gridSize) + new Vector2(i * gridSize / 4, Renderer.Height)).ToPointD());
				Renderer.SetColor(ColorPresets.Gray83);
				g.Stroke();
			}
			for (int i = 0; i < ((int)Math.Floor((double)gridHeight / (double)gridSize)) * 4; i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(gridSize, 0) + new Vector2(0, i * gridSize / 4)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(gridSize, 0) + new Vector2(viewportWidth, i * gridSize / 4)).ToPointD());
				Renderer.SetColor(ColorPresets.Gray83);
				g.Stroke();
			}*/

			// Major grid
			for (int i = 0; i < ((int)Math.Floor((double)gridWidth / (double)gridSize)); i++) {
				g.MoveTo((Client.Local.ViewOrigin - gridOffset - new Vector2(0, gridSize) + new Vector2(i * gridSize, 0)).ToPointD());
				g.LineTo((Client.Local.ViewOrigin - gridOffset + new Vector2(0, gridSize) + new Vector2(i * gridSize, Renderer.Height)).ToPointD());
				Renderer.SetColor(ColorPresets.Gray83);
				g.Stroke();
			}
			for (int i = 0; i < ((int)Math.Floor((double)gridHeight / (double)gridSize)); i++) {
				g.MoveTo((Client.Local.ViewOrigin - gridOffset - new Vector2(gridSize, 0) + new Vector2(0, i * gridSize)).ToPointD());
				g.LineTo((Client.Local.ViewOrigin - gridOffset + new Vector2(gridSize, 0) + new Vector2(viewportWidth, i * gridSize)).ToPointD());
				Renderer.SetColor(ColorPresets.Gray83);
				g.Stroke();
			}

			Client.Local.Draw(gameTime);

			HUD.Draw(gameTime, Renderer, Client.Local.LocalPlayer);

			//Renderer.DrawText(Client.Local.ViewOrigin, string.Format("({0}, {1})", Client.Local.LocalPlayer.CurrentUnit.Position.X, Client.Local.LocalPlayer.CurrentUnit.Position.Y), 20, TextAlign.Left, TextAlign.Top, ColorPresets.White, ColorPresets.Black, null, 0, null);

			if (Client.Local.LocalPlayer.CurrentUnit != null && Client.Local.LocalPlayer.CurrentUnit.Position != Client.Local.LocalPlayer.CurrentUnit.IntendedPosition && Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null) {
				Vector2 intendedPos = new Vector2((float)(Client.Local.LocalPlayer.CurrentUnit.IntendedPosition.X * Renderer.Zoom), (float)(Client.Local.LocalPlayer.CurrentUnit.IntendedPosition.Y * Renderer.Zoom)) - Client.Local.ViewPosition + Client.Local.ViewOrigin;
				g.Save();
				g.SetDash(new double[] { 4, 4 }, 0);
				if (Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null)
					Client.Local.LocalPlayer.CurrentUnit.Actor.Shape.Draw(Renderer, intendedPos, Client.Local.LocalPlayer.CurrentUnit.IntendedDirection, null, new Cairo.Color(0.25, 0.25, 0.25, 0.25), Renderer.GetUnitSize((double)Config.ActorSize / 2) * (1 + 1 * markerAnimationPercent));
				g.MoveTo(Client.Local.LocalPlayer.CurrentUnit.Actor.Position.ToPointD());
				g.LineTo(Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null ? (intendedPos).ToPointD() : Client.Local.LocalPlayer.CurrentUnit.AttackTarget.Position.ToPointD());
				Renderer.SetColor(new Cairo.Color(0.1, 0.1, 0.1, 0.1));
				g.Stroke();
				g.Restore();
			}

			Cairo.Color cursorColor = new Cairo.Color(1, 1, 1);

			foreach (Actor a in Client.Local.Actors) {
				if (a == Client.Local.LocalPlayer.CurrentUnit.Actor)
					continue;
				if (Vector2.Distance(a.Position, cursorPosition) < Renderer.GetUnitSize(Config.ActorSize)) {
					if (Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Enemy)
						cursorColor = new Cairo.Color(1, 0, 0);
					if (Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Friend)
						cursorColor = new Cairo.Color(0, 1, 0);
				}
			}

			if (IsLastActiveState) {
				cursor.Draw(Renderer, cursorPosition, 0, cursorColor, new Cairo.Color(0.1, 0.1, 0.1), 22);
			}
		}
	}
}

