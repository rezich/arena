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
		Vector2 viewPosition = Vector2.Zero;
		Vector2 viewOrigin = Vector2.Zero;
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
				return cursorPosition + viewPosition - viewOrigin;
			}
		}
		int viewMoveSpeed = 16;

		bool isLocalGame = false;

		public override void Initialize() {
			viewPosition = new Vector2(0, 0);
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
			if (cursorPosition.X < viewOrigin.X + edgeScrollSize && !StateManager.Game.CursorVisible)
				viewPosition.X -= viewMoveSpeed;
			if (cursorPosition.X > viewOrigin.X + viewportWidth - edgeScrollSize && !StateManager.Game.CursorVisible)
				viewPosition.X += viewMoveSpeed;
			if (cursorPosition.Y < viewOrigin.Y + edgeScrollSize && !StateManager.Game.CursorVisible)
				viewPosition.Y -= viewMoveSpeed;
			if (cursorPosition.Y > viewOrigin.Y + viewportHeight - edgeScrollSize && !StateManager.Game.CursorVisible)
				viewPosition.Y += viewMoveSpeed;
			if (InputManager.MouseButtonState(MouseButton.Right) == ButtonState.Pressed && cursorPosition.X > HUD.BoxWidth && cursorPosition.X < Renderer.Width - HUD.BoxWidth) {
				markerAnimationDone = gameTime.TotalGameTime + markerAnimationDuration;
				Actor clickedActor = null;
				foreach (Actor a in Client.Local.Actors) {
					if (Vector2.Distance(cursorPosition, a.Position) < Arena.Config.ActorScale) {
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
					viewPosition.Y -= viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Left))
					viewPosition.X -= viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Right))
					viewPosition.X += viewMoveSpeed;
				if (InputManager.KeyDown(Keys.Down))
					viewPosition.Y += viewMoveSpeed;
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
					viewPosition = Client.Local.LocalPlayer.CurrentUnit.Position - new Vector2(viewportWidth / 2, viewportHeight / 2);
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
			}
		}
		public override void Update(GameTime gameTime) {
			if (isLocalGame)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, viewPosition, viewOrigin);
			HUD.Update(gameTime, Client.Local.LocalPlayer);

			markerAnimationPercent = Math.Min(Math.Max(((double)(markerAnimationDone.TotalMilliseconds - gameTime.TotalGameTime.TotalMilliseconds) / (double)markerAnimationDuration.TotalMilliseconds), 0), 1);
		}
		public override void Draw(GameTime gameTime) {
			if (Client.Local.LocalPlayer == null || Client.Local.LocalPlayer.CurrentUnit == null)
				return;
			Renderer.Clear(new Color(0.83, 0.83, 0.83));
			Cairo.Context g = Renderer.Context;

			viewOrigin = new Vector2(Renderer.Width / 10, 0);
			Vector2 gridOffset = new Vector2((float)(viewPosition.X % Arena.Config.ActorScale), (float)(viewPosition.Y % Arena.Config.ActorScale));
			int gridSize = Convert.ToInt32(Arena.Config.ActorScale);
			int gridWidth = viewportWidth + 2 * gridSize;
			int gridHeight = Renderer.Height + 2 * gridSize;

			for (int i = 0; i < ((int)Math.Floor((double)gridWidth / (double)gridSize)); i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(0, gridSize) + new Vector2(i * gridSize, 0)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(0, gridSize) + new Vector2(i * gridSize, Renderer.Height)).ToPointD());
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			for (int i = 0; i < ((int)Math.Floor((double)gridHeight / (double)gridSize)); i++) {
				g.MoveTo((viewOrigin - gridOffset - new Vector2(gridSize, 0) + new Vector2(0, i * gridSize)).ToPointD());
				g.LineTo((viewOrigin - gridOffset + new Vector2(gridSize, 0) + new Vector2(viewportWidth, i * gridSize)).ToPointD());
				g.Color = new Cairo.Color(0.8, 0.8, 0.8);
				g.Stroke();
			}
			Client.Local.Draw(gameTime, g);

			HUD.Draw(gameTime, Renderer, Client.Local.LocalPlayer);

			if (Client.Local.LocalPlayer.CurrentUnit != null && Client.Local.LocalPlayer.CurrentUnit.Position != Client.Local.LocalPlayer.CurrentUnit.IntendedPosition && Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null) {
				g.Save();
				g.SetDash(new double[] { 4, 4 }, 0);
				if (Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null)
					Client.Local.LocalPlayer.CurrentUnit.Actor.Shape.Draw(g, Client.Local.LocalPlayer.CurrentUnit.IntendedPosition - viewPosition + viewOrigin, Client.Local.LocalPlayer.CurrentUnit.IntendedDirection, null, new Cairo.Color(0.25, 0.25, 0.25, 0.25), Arena.Config.ActorScale * (1 + 1 * markerAnimationPercent));
				g.MoveTo(Client.Local.LocalPlayer.CurrentUnit.Actor.Position.ToPointD());
				g.LineTo(Client.Local.LocalPlayer.CurrentUnit.AttackTarget == null ? (Client.Local.LocalPlayer.CurrentUnit.IntendedPosition - viewPosition + viewOrigin).ToPointD() : Client.Local.LocalPlayer.CurrentUnit.AttackTarget.Position.ToPointD());
				g.Color = new Cairo.Color(0.1, 0.1, 0.1, 0.1);
				g.Stroke();
				g.Restore();
			}

			Cairo.Color cursorColor = new Cairo.Color(1, 1, 1);

			foreach (Actor a in Client.Local.Actors) {
				if (a == Client.Local.LocalPlayer.CurrentUnit.Actor)
					continue;
				if (Vector2.Distance(a.Position, cursorPosition) < Arena.Config.ActorScale) {
					if (Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Enemy)
						cursorColor = new Cairo.Color(1, 0, 0);
					if (Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Friend)
						cursorColor = new Cairo.Color(0, 1, 0);
				}
			}

			if (IsLastActiveState) {
				cursor.Draw(g, cursorPosition, 0, cursorColor, new Cairo.Color(0.1, 0.1, 0.1), 22);
			}
		}
	}
}

