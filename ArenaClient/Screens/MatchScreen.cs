using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;
using VGame;
using Arena;

namespace ArenaClient {
	public class MatchScreen : VGame.GameScreen {
		Vector2 cursorPosition;
		Arena.Shapes.Cursor cursor = new Arena.Shapes.Cursor();
		Vector2 viewPosition;
		Vector2 viewOrigin;
		int edgeScrollSize = 32;
		double markerAnimationPercent = 0;
		TimeSpan markerAnimationDuration = TimeSpan.FromSeconds(0.25);
		TimeSpan markerAnimationDone = TimeSpan.Zero;

		int viewportWidth {
			get {
				return Resolution.Width - HUD.BoxWidth * 2;
			}
		}
		int viewportHeight {
			get {
				return Resolution.Height;
			}
		}
		Vector2 cursorWorldPosition {
			get {
				return cursorPosition + viewPosition - viewOrigin;
			}
		}
		int viewMoveSpeed = 16;

		bool isLocalGame = true;

		public MatchScreen() {
			Client.Local = new Client();
			if (isLocalGame) {
				Server.Local = new Server();
			}
			Client.Local.Connect();

			Client.Local.AddPlayer("takua108", 17, Teams.Home, Roles.Runner);

			Server.Local.AddBot(Teams.Away, Roles.Nuker);

			viewPosition = new Vector2(0, 0);
			HUD.Recalculate();
			Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)(Resolution.Width / 2), (int)(Resolution.Height / 2));
		}
		public override void HandleInput(GameTime gameTime, InputState input) {
			cursorPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
			if (cursorPosition.X < viewOrigin.X + edgeScrollSize && !ScreenManager.Game.IsMouseVisible)
				viewPosition.X -= viewMoveSpeed;
			if (cursorPosition.X > viewOrigin.X + viewportWidth - edgeScrollSize && !ScreenManager.Game.IsMouseVisible)
				viewPosition.X += viewMoveSpeed;
			if (cursorPosition.Y < viewOrigin.Y + edgeScrollSize && !ScreenManager.Game.IsMouseVisible)
				viewPosition.Y -= viewMoveSpeed;
			if (cursorPosition.Y > viewOrigin.Y + viewportHeight - edgeScrollSize && !ScreenManager.Game.IsMouseVisible)
				viewPosition.Y += viewMoveSpeed;
			if (input.IsNewMousePress(MouseButtons.Right) && cursorPosition.X > HUD.BoxWidth && cursorPosition.X < Resolution.Width - HUD.BoxWidth) {
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
			if (input.IsKeyDown(Keys.Up))
				viewPosition.Y -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Left))
				viewPosition.X -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Right))
				viewPosition.X += viewMoveSpeed;
			if (input.IsKeyDown(Keys.Down))
				viewPosition.Y += viewMoveSpeed;
			if (input.IsNewKeyPress(Keys.Q)) {
				if (input.IsKeyDown(Keys.LeftShift))
					Client.Local.LevelUp(gameTime, 0);
				else {
					Client.Local.BeginUsingAbility(gameTime, 0);
				}
			}
			if (input.IsNewKeyPress(Keys.W)) {
				if (input.IsKeyDown(Keys.LeftShift))
					Client.Local.LevelUp(gameTime, 1);
				else {
					Client.Local.BeginUsingAbility(gameTime, 1);
				}
			}
			if (input.IsNewKeyPress(Keys.E)) {
				if (input.IsKeyDown(Keys.LeftShift))
					Client.Local.LevelUp(gameTime, 2);
				else {
					Client.Local.BeginUsingAbility(gameTime, 2);
				}
			}
			if (input.IsNewKeyPress(Keys.R)) {
				if (input.IsKeyDown(Keys.LeftShift))
					Client.Local.LevelUp(gameTime, 3);
				else {
					Client.Local.BeginUsingAbility(gameTime, 3);
				}
			}
			if (input.IsNewKeyPress(Keys.Space)) {
				viewPosition = Client.Local.LocalPlayer.CurrentUnit.Position - new Vector2(viewportWidth / 2, viewportHeight / 2);
			}
			if (input.IsNewKeyPress(Keys.F1)) {
				ScreenManager.Game.IsMouseVisible = !ScreenManager.Game.IsMouseVisible;
			}
			if (input.IsNewKeyPress(Keys.F2)) {
				if (Client.Local.IsLocalServer)
					Server.Local.IsDrawing = !Server.Local.IsDrawing;
			}
			base.HandleInput(gameTime, input);
		}
		public override void Update(GameTime gameTime) {
			if (isLocalGame)
				Server.Local.Update(gameTime);
			Client.Local.Update(gameTime, viewPosition, viewOrigin);

			markerAnimationPercent = Math.Min(Math.Max(((double)(markerAnimationDone.TotalMilliseconds - gameTime.TotalGameTime.TotalMilliseconds) / (double)markerAnimationDuration.TotalMilliseconds), 0), 1);
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime) {
			Cairo.Context g = VGame.Renderer.Context;

			viewOrigin = new Vector2(Renderer.Width / 10, 0);
			Vector2 gridOffset = new Vector2((float)(viewPosition.X % Arena.Config.ActorScale), (float)(viewPosition.Y % Arena.Config.ActorScale));
			int gridSize = Convert.ToInt32(Arena.Config.ActorScale);
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
			Client.Local.Draw(gameTime, g);
			if (Client.Local.IsLocalServer && Server.Local.IsDrawing)
				Server.Local.Draw(gameTime, g, viewPosition, viewOrigin);
			/*foreach (Effect e in Effect.List)
				if (e.Height == EffectPosition.BelowActor)
					e.Draw(gameTime, g, LocalPlayer);
			foreach (Actor a in Actor.List) {
				a.DrawUIBelow(gameTime, g, LocalPlayer);
			}
			foreach (Actor a in Actor.List) {
				a.Draw(gameTime, g, LocalPlayer);
			}
			foreach (Actor a in Actor.List) {
				a.DrawUIAbove(gameTime, g, LocalPlayer);
			}
			foreach (Effect e in Effect.List)
				if (e.Height == EffectPosition.AboveActor)
					e.Draw(gameTime, g, LocalPlayer);
					*/

			HUD.Draw(gameTime, g, Client.Local.LocalPlayer);

			if (Client.Local.LocalPlayer.CurrentUnit.Position != Client.Local.LocalPlayer.CurrentUnit.IntendedPosition) {
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
						cursorColor = new Cairo.Color(0, 0, 1);
					if (Client.Local.LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Friend)
						cursorColor = new Cairo.Color(0, 1, 0);
				}
			}


			cursor.Draw(g, cursorPosition, 0, cursorColor, new Cairo.Color(0.1, 0.1, 0.1), 22);
		}
	}
}

