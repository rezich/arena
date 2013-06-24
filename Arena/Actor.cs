using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cairo;

namespace Arena {
	public class Actor {
		public static List<Actor> List = new List<Actor>();
		public Vector2 Position {
			get {
				return Player.Position - _viewPosition + _viewOrigin;
			}
		}
		public double Direction {
			get {
				return Player.Direction;
			}
		}
		public Player Player;
		Vector2 _viewPosition;
		Vector2 _viewOrigin;

		public VGame.IShape Shape;
		
		public Actor() {
		}
		public void Initialize() {
		}
		public void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			_viewPosition = viewPosition;
			_viewOrigin = viewOrigin;
		}
		public virtual void Draw(GameTime gameTime, Context g) {
			Shape.Draw(g, Position, Direction, (Player.Team == Teams.Home ? GameSession.HomeColor1 : GameSession.AwayColor1), (Player.Team == Teams.Home ? GameSession.HomeColor2 : GameSession.AwayColor2), GameSession.ActorScale);
		}
		public void DrawUIBelow(GameTime gameTime, Context g) {
			if (Player.Health < 1)
				return;
			double percent = (double)Player.Health / (double)Player.MaxHealth;
			double size = GameSession.ActorScale + 10;
			Vector2 start = Position + new Vector2(0, (float)-size);
			Vector2 end = Position + new Vector2((float)(Math.Cos(MathHelper.PiOver2) * size), (float)(Math.Sin(MathHelper.PiOver2) * size));
			g.MoveTo(start.X, start.Y);
			g.ArcNegative(Position.X, Position.Y, size, 3 * MathHelper.PiOver2, 3 * MathHelper.PiOver2 - (MathHelper.TwoPi * percent));
			if (percent < 1) {
				g.LineTo(Position.X, Position.Y);
				g.LineTo(start.X, start.Y);
			}
			g.Color = (Player.Team == GameSession.CurrentTeam ? GameSession.HealthColor1 : GameSession.EnemyHealthColor1);
			g.FillPreserve();
			g.Color = (Player.Team == GameSession.CurrentTeam ? GameSession.HealthColor2 : GameSession.EnemyHealthColor2);

			double unit = MathHelper.TwoPi / Player.MaxHealth;
			for (int i = 0; i < Player.Health; i++) {
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				g.MoveTo(Position.X, Position.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = (Player.Team == GameSession.CurrentTeam ? GameSession.HealthColor2 : GameSession.EnemyHealthColor2);
				g.Stroke();
			}

			if (Player.Energy < 1 || Player.Team != GameSession.CurrentTeam)
				return;
			double ePercent = (double)Player.Energy / (double)Player.MaxEnergy;
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

			unit = MathHelper.TwoPi / Player.MaxEnergy;
			for (int i = 0; i < Player.Energy; i++) {
				Vector2 src = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * size), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * size));
				Vector2 dest = Position + new Vector2((float)(Math.Cos(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)), (float)(Math.Sin(3 * MathHelper.PiOver2 - unit * i) * (size + energySize)));
				g.MoveTo(src.X, src.Y);
				g.LineTo(dest.X, dest.Y);
				g.Color = GameSession.EnergyColor2;
				g.Stroke();
			}
		}
		public void DrawUIAbove(GameTime gameTime, Context g) {
			g.SelectFontFace("04b_19", FontSlant.Normal, FontWeight.Bold);
			double textScale = 0.7;
			g.SetFontSize(GameSession.ActorScale * textScale);
			string str = Player.Number.ToString();
			TextExtents ext = g.TextExtents(str);
			Vector2 textPos = new Vector2((float)(Position.X - ext.Width / 2 - ext.XBearing), (float)(Position.Y - ext.Height / 2 - ext.YBearing));
			g.MoveTo(textPos.ToPointD());
			g.SetSourceRGBA(1, 1, 1, 1);
			g.ShowText(str);
			g.MoveTo(textPos.ToPointD());
			g.Color = (Player.Team == Teams.Home ? GameSession.HomeColor2 : GameSession.AwayColor2);
			g.LineWidth = 1;
			g.TextPath(str);
			g.Stroke();
			g.LineWidth = 2.0;
			g.Save();

			double rangeRadius = GameSession.ActorScale * Role.List[Player.Role].AttackRange;
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
}

