using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;

namespace Arena {
	public class Actor {
		public enum Teams {
			Home,
			Away
		}
		public Vector2 Position;
		private Vector2 lastPosition;
		public double Direction = 0;

		public int Health = 10;
		public int MaxHealth = 10;
		public double HealthRegen = 0.001;
		private double healthRegenPart = 0;
		public int Energy = 5;
		public int MaxEnergy = 14;
		public double EnergyRegen = 0.0025;
		private double energyRegenPart = 0;
		public int Number;

		public VGame.IShape Shape;

		public Teams Team;
		
		public Actor() {
		}
		public void Initialize() {
			Health = MaxHealth;
			Number = GameSession.Random.Next(1, 49);
		}
		public void Update(GameTime gameTime) {

		}
		public virtual void Draw(Context g) {
			Shape.Draw(g, Position, Direction, (Team == Teams.Home ? GameSession.HomeColor1 : GameSession.AwayColor1), (Team == Teams.Home ? GameSession.HomeColor2 : GameSession.AwayColor2), GameSession.ActorScale);
		}
		public void DrawUIBelow(Context g) {
			if (Health < 1)
				return;
			double percent = (double)Health / (double)MaxHealth;
			double size = GameSession.ActorScale + 10;
			Vector2 start = Position + new Vector2(0, (float)-size);
			Vector2 end = Position + new Vector2((float)(Math.Cos(MathHelper.PiOver2) * size), (float)(Math.Sin(MathHelper.PiOver2) * size));
			g.MoveTo(start.X, start.Y);
			g.ArcNegative(Position.X, Position.Y, size, 3 * MathHelper.PiOver2, 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * percent));
			if (percent < 1) {
				g.LineTo(Position.X, Position.Y);
				g.LineTo(start.X, start.Y);
			}
			g.Color = (Team == GameSession.CurrentTeam ? GameSession.HealthColor1 : GameSession.EnemyHealthColor1);
			g.FillPreserve();
			g.Color = (Team == GameSession.CurrentTeam ? GameSession.HealthColor2 : GameSession.EnemyHealthColor2);

			double unit = MathHelper.TwoPi / MaxHealth;
			for (int i = 0; i < Health; i++) {
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				g.MoveTo(Position.X, Position.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = (Team == GameSession.CurrentTeam ? GameSession.HealthColor2 : GameSession.EnemyHealthColor2);
				g.Stroke();
			}

			if (Energy < 1 || Team != GameSession.CurrentTeam)
				return;
			double ePercent = (double)Energy / (double)MaxEnergy;
			g.MoveTo(Position.X, Position.Y - size);
			double energy = 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * ePercent);
			int energySize = 5;
			g.ArcNegative(Position.X, Position.Y, size, 3 * MathHelper.PiOver2, energy);
			g.LineTo(Position.X + Math.Cos(energy) * (energySize + size), Position.Y + Math.Sin(energy) * (size + energySize));
			g.Arc(Position.X, Position.Y, size + energySize, energy, 3 * MathHelper.PiOver2);
			g.ClosePath();
			g.Color = GameSession.EnergyColor1;
			g.FillPreserve();
			g.Color = GameSession.EnergyColor2;
			g.Stroke();

			unit = MathHelper.TwoPi / MaxEnergy;
			for (int i = 0; i < Energy; i++) {
				Vector2 src = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)));
				g.MoveTo(src.X, src.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = GameSession.EnergyColor2;
				g.Stroke();
			}
		}
		public void DrawUIAbove(Context g) {
			g.SelectFontFace("04b_19", FontSlant.Normal, FontWeight.Bold);
			double textScale = 0.7;
			g.SetFontSize(GameSession.ActorScale * textScale);
			string str = Number.ToString();
			TextExtents ext = g.TextExtents(str);
			Vector2 textPos = new Vector2((float)(Position.X - ext.Width / 2 - ext.XBearing), (float)(Position.Y - ext.Height / 2 - ext.YBearing));
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(1, 1, 1, 1);
			g.ShowText(str);
			g.MoveTo(textPos.ToPointD());
			g.Color = (Team == Teams.Home ? GameSession.HomeColor2 : GameSession.AwayColor2);
			g.LineWidth = 1;
			g.TextPath(str);
			g.Stroke();
			g.LineWidth = 2.0;
			g.Save();
			double[] dash = new double[] { MathHelper.Pi * 3, MathHelper.Pi };
			g.SetDash(dash, 0);
			g.Arc(Position.X, Position.Y, GameSession.ActorScale + 25, 0, MathHelper.TwoPi);
			g.SetSourceRGBA(0, 0, 0, 0.25);
			g.Stroke();
			g.Restore();
		}
	}
	public class Runner : Actor {
		public Runner() {
			MaxHealth = 15;
			Shape = new Shapes.Runner();
			Initialize();
		}
	}
	public class Grappler : Actor {
		public Grappler() {
			MaxHealth = 25;
			Shape = new Shapes.Grappler();
			Initialize();
		}
	}
	public class Tank : Actor {
		public Tank() {
			MaxHealth = 35;
			Shape = new Shapes.Tank();
			Initialize();
		}
	}
	public class Nuker : Actor {
		public Nuker() {
			MaxHealth = 10;
			Shape = new Shapes.Nuker();
			Initialize();
		}
	}
}

