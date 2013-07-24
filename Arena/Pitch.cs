using System;
using System.Collections.Generic;
using Cairo;
using VGame;

namespace Arena {
	public class Pitch {

		public List<Polygon> Terrain;
		public List<Tuple<Line, Color>> Decorations;
		public const int Width = 1500;
		public const int BaseWidth = 300;
		public const int LaneWidth = 150;

		public Pitch() {
			Initialize();
		}

		public void Initialize() {
			//int laneWidth = 177;
			int gutter = BaseWidth / 2 - LaneWidth / 2;
			Vector2 baseCorner = new Vector2(BaseWidth, Width - BaseWidth);
			Vector2 _midTop = baseCorner.AddLengthDir(LaneWidth / 2, -(MathHelper.PiOver4 * 3));
			Vector2 _midBot = baseCorner.AddLengthDir(LaneWidth / 2, MathHelper.PiOver4);
			Vector2 _center = new Vector2(Width / 2, Width / 2);
			Vector2 _centerTop = _center.AddLengthDir(LaneWidth / 2, -(MathHelper.PiOver4 * 3));
			Vector2 _centerBot = _center.AddLengthDir(LaneWidth / 2, MathHelper.PiOver4);
			VGame.Point midTop = new VGame.Point((int)Math.Round(_midTop.X), (int)Math.Round(_midTop.Y));
			VGame.Point midBot = new VGame.Point((int)Math.Round(_midBot.X), (int)Math.Round(_midBot.Y));
			VGame.Point centerTop = new VGame.Point((int)Math.Round(_centerTop.X), (int)Math.Round(_centerTop.Y));
			VGame.Point centerBot = new VGame.Point((int)Math.Round(_centerBot.X), (int)Math.Round(_centerBot.Y));
			VGame.Point center = new VGame.Point(Width / 2, Width / 2);
			Terrain = new List<Polygon>() {
				// base
				new Polygon(0, Width, BaseWidth, Width, 0, Width - BaseWidth),
				new Polygon(BaseWidth, Width - BaseWidth, BaseWidth, Width, 0, Width - BaseWidth),
				// hardlane
				new Polygon(gutter, Width - BaseWidth, BaseWidth / 2 + LaneWidth / 2, Width - BaseWidth, gutter, gutter),
				new Polygon(BaseWidth / 2 + LaneWidth / 2, Width - BaseWidth, gutter, gutter, gutter + LaneWidth, gutter + LaneWidth),
				// safelane
				new Polygon(BaseWidth, Width - BaseWidth + gutter, BaseWidth, Width - gutter, Width - gutter, Width - gutter),
				new Polygon(BaseWidth, Width - BaseWidth + gutter, Width - gutter - LaneWidth, Width - gutter - LaneWidth, Width - gutter, Width - gutter),
				// midlane
				/*new Polygon(BaseWidth - gutter, Width - BaseWidth, BaseWidth, Width - BaseWidth, midTop.X, midTop.Y),
				new Polygon(BaseWidth, Width - BaseWidth + gutter, BaseWidth, Width - BaseWidth, midBot.X, midBot.Y),
				new Polygon(midTop, midBot, centerTop),
				new Polygon(midBot, centerTop, centerBot)*/
			};
			List<Polygon> flip = new List<Polygon>(Terrain);
			foreach (Polygon p in flip) {
				p.RotateAbout(new VGame.Point(Width / 2, Width / 2), MathHelper.Pi);
				Terrain.Add(p);
			}
			Decorations = new List<Tuple<Line, Color>>() {
				new Tuple<Line, Color>(new Line(0, 0, Width, Width), ColorPresets.Red)
			};
		}
		public void Draw(Renderer renderer) {
			int polys = 0;
			Context g = renderer.Context;
			int padding = 32;
			VGame.Rectangle r = new VGame.Rectangle((int)Math.Round(Client.Local.ViewOrigin.X) - padding, (int)Math.Round(Client.Local.ViewOrigin.Y) - padding, Client.Local.Game.Renderer.Width + 2 * padding, Client.Local.Game.Renderer.Height + 2 * padding);
			double scale = renderer.GetUnitSize();
			foreach (Polygon p in Terrain) {
				Polygon sp = p.ScaleAndOffset(scale, -Client.Local.ViewPosition + Client.Local.ViewOrigin);
				bool draw = false;
				foreach (Line l in sp.Lines) {
					if (r.IntersectsLine(new Vector2((float)(l.Point1.X), (float)(l.Point1.Y)), new Vector2((float)(l.Point2.X), (float)(l.Point2.Y)))) {
						draw = true;
						break;
					}
				}
				if (draw || sp.Contains(r)) {
					p.Draw(renderer, -Client.Local.ViewPosition + Client.Local.ViewOrigin, scale);
					polys++;
				}
				/*g.MoveTo(r.X, r.Y);
				g.LineTo(r.X + r.Width, r.Y);
				g.LineTo(r.X + r.Width, r.Y + r.Height);
				g.LineTo(r.X, r.Y + r.Height);
				g.ClosePath();
				renderer.StrokeAndFill(new Color(0.2, 0.5, 0.2, 0.5), new Color(0.2, 0.5, 0.2, 0.25));*/
			}
			foreach (Tuple<Line, Color> t in Decorations) {
				if (r.IntersectsLine(t.Item1.Point1, t.Item1.Point2)) {
					g.MoveTo((new Vector2((float)(t.Item1.Point1.X * scale), (float)(t.Item1.Point1.Y * scale)) - Client.Local.ViewPosition + Client.Local.ViewOrigin).ToPointD());
					g.LineTo((new Vector2((float)(t.Item1.Point2.X * scale), (float)(t.Item1.Point2.Y * scale)) - Client.Local.ViewPosition + Client.Local.ViewOrigin).ToPointD());
					renderer.SetColor(new Color(t.Item2.R, t.Item2.G, t.Item2.B, 0.4));
					g.Stroke();
				}
			}
			//renderer.DrawText(Client.Local.ViewOrigin, string.Format("polys: {0}", polys), 20, TextAlign.Left, TextAlign.Top, ColorPresets.White, ColorPresets.Black, null, 0, null);
		}
	}
}

