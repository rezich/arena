using System;
using Microsoft.Xna.Framework;
using Cairo;

namespace Arena.Shapes {
	public class Runner : VGame.Shape {
		public override void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			Vector2 tip = position.AddLengthDir(scale, direction);
			Vector2 rightLeg = position.AddLengthDir(scale, direction + 3 * MathHelper.PiOver4);
			Vector2 rear = position.AddLengthDir(scale / 3, direction + MathHelper.Pi);
			Vector2 leftLeg = position.AddLengthDir(scale, direction - 3 * MathHelper.PiOver4);
			g.MoveTo(tip.ToPointD());
			g.LineTo(rightLeg.ToPointD());
			g.LineTo(rear.ToPointD());
			g.LineTo(leftLeg.ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
	}
	public class Grappler : VGame.Shape {
		public override void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
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
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
	}
	public class Tank : VGame.Shape {
		public override void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
			double front = 3 * MathHelper.PiOver4 / 2;
			double back = 3 * MathHelper.PiOver4;
			g.MoveTo(position.AddLengthDir(scale, direction).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + front).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction + back).ToPointD());
			g.Arc(position.X, position.Y, scale, direction + back, direction - back);
			g.LineTo(position.AddLengthDir(scale, direction - back).ToPointD());
			g.LineTo(position.AddLengthDir(scale, direction - front).ToPointD());
			g.ClosePath();
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
	}
	public class Nuker : VGame.Shape {
		public override void Draw(Context g, Vector2 position, double direction, Cairo.Color? fillColor, Cairo.Color? strokeColor, double scale) {
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
			VGame.Util.StrokeAndFill(g, fillColor, strokeColor);
		}
	}
}

