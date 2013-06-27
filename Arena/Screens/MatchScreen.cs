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
		CreepController NeutralController = new CreepController(Teams.Neutral);

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
			LocalPlayer.MakePlayerUnit(new Vector2(144, 144));
			LocalPlayer.PlayerUnit.LevelUp(0);
			LocalPlayer.PlayerUnit.LevelUp(1);

			Bot bot1 = new Bot(Teams.Away, Roles.Nuker);
			bot1.MakePlayerUnit(new Vector2(300, 300));

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
						if (LocalPlayer.PlayerUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Enemy) {
							clickedActor = a;
							break;
						}
					}
				}
				if (clickedActor != null)
					LocalPlayer.CurrentUnit.AttackTarget = clickedActor;
				else {
					LocalPlayer.CurrentUnit.AttackTarget = null;
					LocalPlayer.CurrentUnit.IntendedPosition = cursorWorldPosition;
				}
			}
			/*if (input.IsNewMousePress(MouseButtons.Left) && cursorPosition.X > HUD.BoxWidth && cursorPosition.X < Resolution.Width - HUD.BoxWidth) {
				Effect e = new Effect(cursorWorldPosition, EffectPosition.BelowActor, new Shapes.AutoAttackBeam());
			}*/
			if (input.IsKeyDown(Keys.Up))
				viewPosition.Y -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Left))
				viewPosition.X -= viewMoveSpeed;
			if (input.IsKeyDown(Keys.Right))
				viewPosition.X += viewMoveSpeed;
			if (input.IsKeyDown(Keys.Down))
				viewPosition.Y += viewMoveSpeed;
			if (input.IsNewKeyPress(Keys.Q)) {
				LocalPlayer.CurrentUnit.UseAbility(0);
			}
			if (input.IsNewKeyPress(Keys.W)) {
				LocalPlayer.CurrentUnit.UseAbility(1);
			}
			if (input.IsNewKeyPress(Keys.E)) {
				LocalPlayer.CurrentUnit.UseAbility(2);
			}
			if (input.IsNewKeyPress(Keys.R)) {
				LocalPlayer.CurrentUnit.UseAbility(3);
			}
			if (input.IsNewKeyPress(Keys.Space)) {
				viewPosition = LocalPlayer.CurrentUnit.Position - new Vector2(viewportWidth / 2, viewportHeight / 2);
			}
			base.HandleInput(input);
		}
		public override void Update(GameTime gameTime) {
			foreach (Player p in Player.List)
				p.Update(gameTime);
			foreach (Unit u in Unit.List)
				u.Update(gameTime);
			foreach (Actor a in Actor.List)
				a.Update(gameTime, viewPosition, viewOrigin);
			foreach (Effect e in Effect.List)
				e.Update(gameTime, viewPosition, viewOrigin);
			Effect.Cleanup();
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
			foreach (Effect e in Effect.List)
				if (e.Height == EffectPosition.BelowActor)
					e.Draw(gameTime, g);
			foreach (Actor a in Actor.List) {
				a.DrawUIBelow(gameTime, g);
			}
			foreach (Actor a in Actor.List) {
				a.Draw(gameTime, g);
			}
			foreach (Actor a in Actor.List) {
				a.DrawUIAbove(gameTime, g);
			}
			foreach (Effect e in Effect.List)
				if (e.Height == EffectPosition.AboveActor)
					e.Draw(gameTime, g);

			HUD.Draw(gameTime, g, LocalPlayer);

			if (LocalPlayer.CurrentUnit.AttackTarget == null && LocalPlayer.CurrentUnit.Position != LocalPlayer.CurrentUnit.IntendedPosition) {
				g.Save();
				g.SetDash(new double[] { 4, 4 }, 0);
				LocalPlayer.CurrentUnit.Actor.Shape.Draw(g, LocalPlayer.CurrentUnit.IntendedPosition - viewPosition + viewOrigin, LocalPlayer.CurrentUnit.IntendedDirection, null, new Cairo.Color(0.25, 0.25, 0.25, 0.25), Arena.GameSession.ActorScale);
				g.Restore();
			}

			Cairo.Color cursorColor = new Cairo.Color(1, 1, 1);
			foreach (Actor a in Actor.List) {
				if (a == LocalPlayer.CurrentUnit.Actor)
					continue;
				if (Vector2.Distance(a.Position, cursorPosition) < Arena.GameSession.ActorScale) {
					if (LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Enemy)
						cursorColor = new Cairo.Color(0, 0, 1);
					if (LocalPlayer.CurrentUnit.AttitudeTowards(a.Unit.Owner) == Attitude.Friend)
						cursorColor = new Cairo.Color(0, 1, 0);
				}
			}

			cursor.Draw(g, cursorPosition, 0, cursorColor, new Cairo.Color(0.1, 0.1, 0.1), 22);
		}
	}
}

