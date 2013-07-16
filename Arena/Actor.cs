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
		public override void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			base.Update(gameTime, viewPosition, viewOrigin);
		}
		public override void Draw(GameTime gameTime, Context g, Player localPlayer) {
			Shape.Draw(g, Position, Direction, (Unit.Team == Teams.Home ? Arena.Config.HomeColor1 : Arena.Config.AwayColor1), (Unit.Team == Teams.Home ? Arena.Config.HomeColor2 : Arena.Config.AwayColor2), Arena.Config.ActorScale);
			foreach (Ability a in Unit.Abilities)
				a.Draw(gameTime, g);
		}
		public void DrawUIBelow(GameTime gameTime, Context g, Player localPlayer) {
			if (Unit.Health < 1)
				return;
			double percent = (double)Unit.Health / (double)Unit.MaxHealth;
			double size = Arena.Config.ActorScale + 10;
			Vector2 start = Position + new Vector2(0, (float)-size);
			/*Vector2 end = Position + new Vector2((float)(Math.Cos(MathHelper.PiOver2) * size), (float)(Math.Sin(MathHelper.PiOver2) * size));*/
			g.MoveTo(start.X, start.Y);
			g.ArcNegative(Position.X, Position.Y, size, 3 * MathHelper.PiOver2, 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * percent));
			if (percent < 1) {
				g.LineTo(Position.X, Position.Y);
				g.LineTo(start.X, start.Y);
			}
			g.Color = (Unit.Team == localPlayer.Team ? Arena.Config.HealthColor1 : Arena.Config.EnemyHealthColor1);
			g.FillPreserve();
			g.Color = (Unit.Team == localPlayer.Team ? Arena.Config.HealthColor2 : Arena.Config.EnemyHealthColor2);

			double unit = MathHelper.TwoPi / Unit.MaxHealth;
			for (int i = 0; i < Unit.Health; i++) {
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				g.MoveTo(Position.X, Position.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = (Unit.Team == localPlayer.Team ? Arena.Config.HealthColor2 : Arena.Config.EnemyHealthColor2);
				g.Stroke();
			}

			if (Unit.Energy < 1 || Unit.Team != localPlayer.Team)
				return;
			double ePercent = (double)Unit.Energy / (double)Unit.MaxEnergy;
			g.MoveTo(Position.X, Position.Y - size);
			double energy = 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * ePercent);
			int energySize = 5;
			g.ArcNegative(Position.X, Position.Y, size, 3 * MathHelper.PiOver2, energy);
			g.LineTo(Position.X + Math.Cos(energy) * (energySize + size), Position.Y + Math.Sin(energy) * (size + energySize));
			g.Arc(Position.X, Position.Y, size + energySize, energy, 3 * MathHelper.PiOver2);
			g.ClosePath();
			g.Color = Arena.Config.EnergyColor1;
			g.FillPreserve();
			g.Color = Arena.Config.EnergyColor2;
			g.Stroke();

			unit = MathHelper.TwoPi / Unit.MaxEnergy;
			for (int i = 0; i < Unit.Energy; i++) {
				Vector2 src = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)));
				g.MoveTo(src.X, src.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = Arena.Config.EnergyColor2;
				g.Stroke();
			}
		}
		public void DrawUIAbove(GameTime gameTime, Context g, Player localPlayer) {
			g.SelectFontFace("04b_19", FontSlant.Normal, FontWeight.Bold);
			double textScale = 0.7;

			if (Unit.Owner is Player) {
				g.SetFontSize(Arena.Config.ActorScale * textScale);
				string str = ((Player)Unit.Owner).Number.ToString();
				TextExtents ext = g.TextExtents(str);
				Vector2 textPos = new Vector2((float)(Position.X - ext.Width / 2 - ext.XBearing), (float)(Position.Y - ext.Height / 2 - ext.YBearing));
				g.MoveTo(textPos.ToPointD());
				g.SetSourceRGBA(1, 1, 1, 1);
				g.ShowText(str);
				g.MoveTo(textPos.ToPointD());
				g.Color = (Unit.Team == Teams.Home ? Arena.Config.HomeColor2 : Arena.Config.AwayColor2);
				g.LineWidth = 1;
				g.TextPath(str);
				g.Stroke();
				g.LineWidth = 2.0;
			}
			g.Save();

			double rangeRadius = Arena.Config.ActorScale * Unit.AttackRange;
			double rangeCircum = 2 * MathHelper.Pi * rangeRadius;
			double start = gameTime.TotalGameTime.TotalSeconds / MathHelper.TwoPi * 1;
			double[] dash = new double[] { rangeCircum / 80, rangeCircum / 120 };
			g.SetDash(dash, 0);
			g.LineWidth = 2;
			g.Arc(Position.X, Position.Y, rangeRadius, start, start + MathHelper.TwoPi);
			g.SetSourceRGBA(0, 0, 0, 0.1);
			g.Stroke();
			g.Restore();
		}
	}
	public abstract class Drawable {
		public Vector2 Position {
			get {
				return WorldPosition - _viewPosition + _viewOrigin;
			}
		}
		public abstract Vector2 WorldPosition { get; set; }
		public abstract double Direction { get; set; }
		public VGame.Shape Shape;
		protected Vector2 _viewPosition;
		protected Vector2 _viewOrigin;
		public bool ToBeRemoved;

		public virtual void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			_viewPosition = viewPosition;
			_viewOrigin = viewOrigin;
		}
		public abstract void Draw(GameTime gameTime, Context g, Player localPlayer);
	}
}

