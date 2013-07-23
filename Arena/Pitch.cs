using System;
using System.Collections.Generic;
using Cairo;
using VGame;

namespace Arena {
	public class Pitch {

		public List<Line> Terrain;
		public List<Tuple<Line, Color>> Decorations;
		public const int Width = 100;

		public Pitch() {
			Initialize();
		}

		public void Initialize() {
			Terrain = new List<Line>() {
				new Line(0, 0, Width, 0),
				new Line(Width, 0, Width, Width),
				new Line(Width, Width, 0, Width),
				new Line(0, Width, 0, 0)
			};
			Decorations = new List<Tuple<Line, Color>>() {
				new Tuple<Line, Color>(new Line(0, 0, Width, Width), ColorPresets.Red)
			};
		}
		public void Draw(Renderer renderer, Vector2 viewPosition, Vector2 viewOrigin) {
			Context g = renderer.Context;
			VGame.Rectangle r = new VGame.Rectangle((int)viewPosition.X, (int)viewPosition.Y, Client.Local.Game.Renderer.Width, Client.Local.Game.Renderer.Height);
			foreach (Line l in Terrain) {
				if (r.IntersectsLine(l.Point1, l.Point2)) {
					g.MoveTo((l.Point1 - viewPosition).ToPointD());
					g.LineTo((l.Point2 - viewPosition).ToPointD());
					renderer.SetColor(ColorPresets.Gray50);
					g.Stroke();
				}
			}
			foreach (Tuple<Line, Color> t in Decorations) {
				if (r.IntersectsLine(t.Item1.Point1, t.Item1.Point2)) {
					g.MoveTo((t.Item1.Point1 - viewPosition).ToPointD());
					g.LineTo((t.Item1.Point2 - viewPosition).ToPointD());
					renderer.SetColor(t.Item2);
					g.Stroke();
				}
			}
		}
	}
}

