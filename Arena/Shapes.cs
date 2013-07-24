using System;
using Cairo;
using VGame;

namespace Arena.Shapes {
	public class Runner : VGame.Shape {
		public Runner() : base() {
			AnimationSpeed = 0.05;
			AnimationType = AnimationType.Bounce;
		}
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			Vector2 tip = position.AddLengthDir(scale, direction);
			Vector2 rightLeg = position.AddLengthDir(scale, direction + 3 * MathHelper.PiOver4 + (MathHelper.PiOver4 / 7 * AnimationProgress));
			Vector2 rear = position.AddLengthDir(scale / 3, direction + MathHelper.Pi);
			Vector2 leftLeg = position.AddLengthDir(scale, direction - 3 * MathHelper.PiOver4 - (MathHelper.PiOver4 / 7  * AnimationProgress));
			g.MoveTo(tip.ToPointD());
			g.LineTo(rightLeg.ToPointD());
			g.LineTo(rear.ToPointD());
			g.LineTo(leftLeg.ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
	}
	public class Grappler : VGame.Shape {
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			double armWidth = (double)scale / 3.5;

			Vector2 tip = position.AddLengthDir(scale, direction);
			g.MoveTo(position.AddLengthDir(armWidth / 2, direction - MathHelper.PiOver2).ToPointD());
			g.LineTo(tip.AddLengthDir(armWidth / 2, direction - MathHelper.PiOver2).ToPointD());
			g.LineTo(tip.AddLengthDir(armWidth / 2, direction + MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth / 2, direction + MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 2, direction + MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 2, direction + MathHelper.PiOver2).AddLengthDir(scale / 2, direction).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 2, direction + MathHelper.PiOver2).AddLengthDir(scale / 2, direction).AddLengthDir(armWidth, direction + MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 3, direction + MathHelper.PiOver2).AddLengthDir(armWidth, direction + MathHelper.Pi).ToPointD());
			Vector2 arc = position.AddLengthDir(armWidth / 2, direction + MathHelper.Pi);
			g.Arc(arc.X, arc.Y, armWidth * 3, direction - 3 * MathHelper.PiOver2, direction - MathHelper.PiOver2);
			g.LineTo(position.AddLengthDir(armWidth * 2, direction - MathHelper.PiOver2).AddLengthDir(scale / 2, direction).AddLengthDir(armWidth, direction - MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 2, direction - MathHelper.PiOver2).AddLengthDir(scale / 2, direction).ToPointD());
			g.LineTo(position.AddLengthDir(armWidth * 2, direction - MathHelper.PiOver2).ToPointD());

			g.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
	}
	public class Tank : VGame.Shape {
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			double front = 3 * MathHelper.PiOver4 / 2;
			double back = 3 * MathHelper.PiOver4;
			g.MoveTo(position.AddLengthDir(scale, direction).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + front).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + back).ToPointD());
			g.Arc(position.X, position.Y, scale, direction + back, direction - back);
			g.LineTo(position.AddLengthDir(scale, direction - back).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction - front).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
	}
	public class Nuker : VGame.Shape {
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			double front = MathHelper.PiOver2;
			double back = 13 * MathHelper.PiOver4 / 4;
			g.MoveTo(position.AddLengthDir(scale, direction).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + front).ToPointD());
			g.LineTo(position.AddLengthDir(scale * 0.35, direction + front).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + back).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction - back).ToPointD());
			g.LineTo(position.AddLengthDir(scale * 0.35, direction - front).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction - front).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
	}
	public class Cursor : VGame.Shape {
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			g.MoveTo(position.ToPointD());
			g.LineTo(position.AddLengthDir(scale, MathHelper.PiOver2).ToPointD());
			g.LineTo(position.AddLengthDir(scale, MathHelper.PiOver4 * 9).ToPointD());
			g.ClosePath();
			renderer.StrokeAndFill(fillColor, strokeColor);
		}
	}
	public class AutoAttackBeam : VGame.Shape {
		public double Distance = 100;
		public override void Draw(Renderer renderer, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Context g = renderer.Context;
			if (strokeColor.HasValue) {
				g.MoveTo(position.ToPointD());
				g.LineTo(position.AddLengthDir(Distance, direction).ToPointD());
				renderer.SetColor((Cairo.Color)strokeColor);
				g.Stroke();
			}
		}
	}
}

