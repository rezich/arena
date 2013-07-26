using System;
using System.Collections;
using System.Collections.Generic;
using Cairo;
using VGame;

namespace Arena {
	public class Actor : Drawable {
		public override Vector2 WorldPosition {
			get {
				return Unit.Position;
			}
			set {
			}
		}
		public override double Direction {
			get {
				return Unit.Direction;
			}
			set {
			}
		}
		public Unit Unit;
		
		public Actor(Unit unit, VGame.Shape shape) {
			Unit = unit;
			Shape = shape;
		}
		public void Initialize() {
		}
		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime, Renderer renderer, Player localPlayer, Vector2 offset) {
			base.Draw(gameTime, renderer, localPlayer, offset);
			Context g = renderer.Context;
			Shape.Draw(renderer, GetPosition(renderer, offset), Direction, (Unit.Team == Teams.Home ? Arena.Config.HomeColor1 : Arena.Config.AwayColor1), (Unit.Team == Teams.Home ? Arena.Config.HomeColor2 : Arena.Config.AwayColor2), renderer.GetUnitSize((double)Arena.Config.ActorSize / 2));
			foreach (Ability a in Unit.Abilities)
				a.Draw(gameTime, g);
		}
		public void DrawUIBelow(GameTime gameTime, Renderer renderer, Player localPlayer, Vector2 offset) {
			Vector2 position = GetPosition(renderer, offset);
			Context g = renderer.Context;
			if (Unit.Health < 1)
				return;
			double percent = (double)Unit.Health / (double)Unit.MaxHealth;
			double size = renderer.GetUnitSize((double)Arena.Config.ActorSize / 2) * 1.25;
			Vector2 start = position + new Vector2(0, (float)-size);
			g.MoveTo(start.X, start.Y);
			g.ArcNegative(position.X, position.Y, size, 3 * MathHelper.PiOver2, 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * percent));
			if (percent < 1) {
				g.LineTo(position.X, position.Y);
				g.LineTo(start.X, start.Y);
			}
			renderer.SetColor(Unit.Team == localPlayer.Team ? Arena.Config.HealthColor1 : Arena.Config.EnemyHealthColor1);
			g.FillPreserve();
			renderer.SetColor(Unit.Team == localPlayer.Team ? Arena.Config.HealthColor2 : Arena.Config.EnemyHealthColor2);

			double unit = MathHelper.TwoPi / Unit.MaxHealth;
			for (int i = 0; i < Unit.Health; i++) {
				Vector2 dest = position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				g.MoveTo(position.X, position.Y);
				g.LineTo(dest.X, dest.Y);
				renderer.SetColor(Unit.Team == localPlayer.Team ? Arena.Config.HealthColor2 : Arena.Config.EnemyHealthColor2);
				g.Stroke();
			}

			if (Unit.Energy < 1 || Unit.Team != localPlayer.Team)
				return;
			double ePercent = (double)Unit.Energy / (double)Unit.MaxEnergy;
			g.MoveTo(position.X, position.Y - size);
			double energy = 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * ePercent);
			double energySize = size / 7;
			g.ArcNegative(position.X, position.Y, size, 3 * MathHelper.PiOver2, energy);
			g.LineTo(position.X + Math.Cos(energy) * (energySize + size), position.Y + Math.Sin(energy) * (size + energySize));
			g.Arc(position.X, position.Y, size + energySize, energy, 3 * MathHelper.PiOver2);
			g.ClosePath();
			renderer.SetColor(Arena.Config.EnergyColor1);
			g.FillPreserve();
			renderer.SetColor(Arena.Config.EnergyColor2);
			g.Stroke();

			unit = MathHelper.TwoPi / Unit.MaxEnergy;
			for (int i = 0; i < Unit.Energy; i++) {
				Vector2 src = position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				Vector2 dest = position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)));
				g.MoveTo(src.X, src.Y);
				g.LineTo(dest.X, dest.Y);
				renderer.SetColor(Arena.Config.EnergyColor2);
				g.Stroke();
			}
		}
		public void DrawUIAbove(GameTime gameTime, Renderer renderer, Player localPlayer, Vector2 offset) {
			Context g = renderer.Context;
			Vector2 position = GetPosition(renderer, offset);

			renderer.DrawText(position, ((Player)Unit.Owner).Number.ToString(), renderer.GetUnitSize((double)Arena.Config.ActorSize / 2) * 0.7, TextAlign.Center, TextAlign.Middle, ColorPresets.White, (Unit.Team == Teams.Home ? Arena.Config.HomeColor2 : Arena.Config.AwayColor2), null, 0, "chunky_aa");

			g.Save();

			double rangeRadius = renderer.GetUnitSize() * Unit.AttackRange;
			double rangeCircum = 2 * MathHelper.Pi * rangeRadius;
			double start = gameTime.TotalGameTime.TotalSeconds / MathHelper.TwoPi * 1;
			double[] dash = new double[] { rangeCircum / 80, rangeCircum / 120 };
			g.SetDash(dash, 0);
			g.LineWidth = 2;
			g.Arc(position.X, position.Y, rangeRadius, start, start + MathHelper.TwoPi);
			g.SetSourceRGBA(0, 0, 0, 0.1);
			g.Stroke();
			g.Restore();
		}
	}
	public abstract class Drawable {
		public abstract Vector2 WorldPosition { get; set; }
		public abstract double Direction { get; set; }
		public VGame.Shape Shape;
		protected double zoom;
		public bool ToBeRemoved;

		public virtual void Update(GameTime gameTime) {
			Shape.Update(gameTime);
		}
		public virtual void Draw(GameTime gameTime, Renderer renderer, Player localPlayer, Vector2 offset) {
			zoom = renderer.Zoom;
		}
		public Vector2 GetPosition(Renderer renderer, Vector2 offset) {
			return new Vector2((float)(WorldPosition.X * renderer.Zoom), (float)(WorldPosition.Y * renderer.Zoom)) + offset;
		}
	}
}

