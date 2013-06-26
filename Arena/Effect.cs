using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Cairo;

namespace Arena {
	public enum EffectPosition {
		BelowActor,
		AboveActor
	}
	public abstract class Effect : Drawable {
		public static List<Effect> List = new List<Effect>();
		protected Vector2 _position;
		protected double _direction;
		public EffectPosition Height;
		public override Vector2 WorldPosition {
			get {
				return _position;
			}
			set {
				_position = value;
			}
		}
		public override double Direction {
			get {
				return _direction;
			}
			set {
				_direction = value;
			}
		}
		public abstract TimeSpan Duration { get; }
		public abstract Cairo.Color? FillColor { get; }
		public abstract Cairo.Color? StrokeColor { get; }

		public TimeSpan ExpirationTime;
		public double LifePercent = 0;

		public Effect(GameTime gameTime, Vector2 position, EffectPosition height, VGame.IShape shape) {
			_position = position;
			Height = height;
			Shape = shape;
			ExpirationTime = gameTime.TotalGameTime + Duration;
			List.Add(this);
		}

		public abstract void OnUpdate(GameTime gameTime);

		public override void Update(GameTime gameTime, Vector2 viewPosition, Vector2 viewOrigin) {
			base.Update(gameTime, viewPosition, viewOrigin);
			LifePercent = (double)(ExpirationTime.TotalMilliseconds - gameTime.TotalGameTime.TotalMilliseconds) / (double)Duration.TotalMilliseconds;
			OnUpdate(gameTime);
			if (gameTime.TotalGameTime >= ExpirationTime)
				ToBeRemoved = true;
		}
		public override void Draw(GameTime gameTime, Context g) {
			Shape.Draw(g, Position, Direction, FillColor, StrokeColor, GameSession.ActorScale);
		}
		public override void Remove() {
			List.Remove(this);
		}

		public static void Cleanup() {
			for (int i = 0; i < List.Count; i++) {
				if (List[i].ToBeRemoved)
					List[i].Remove();
			}
		}
	}
}

namespace Arena.Effects {
	public class AutoAttackBeam : Effect {
		public override TimeSpan Duration {
			get {
				return TimeSpan.FromSeconds(0.5);
			}
		}
		public override Cairo.Color? FillColor {
			get {
				return null;
			}
		}
		public override Cairo.Color? StrokeColor {
			get {
				return new Cairo.Color(0, 0, 0.5, LifePercent);
			}
		}

		public AutoAttackBeam(GameTime gameTime, Vector2 origin, Vector2 destination) : base(gameTime, origin, EffectPosition.BelowActor, new Shapes.AutoAttackBeam()) {
			((Shapes.AutoAttackBeam)Shape).Distance = Vector2.Distance(origin, destination);
			Direction = Math.Atan2(destination.Y - origin.Y, destination.X - origin.X);
		}

		public override void OnUpdate(GameTime gameTime) {
		}
	}
}
